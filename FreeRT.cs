/*
*  < ----- End-User License Agreement ----->
*  
*  You may not copy, modify, merge, publish, distribute, sublicense, or sell copies of this software without the developer’s consent.
*
*  THIS SOFTWARE IS PROVIDED BY IIIaKa AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
*  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS 
*  BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
*  GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
*  LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*
*  Developer: IIIaKa
*      https://t.me/iiiaka
*      Discord: @iiiaka
*      https://github.com/IIIaKa
*      https://umod.org/user/IIIaKa
*      https://codefling.com/iiiaka
*      https://lone.design/vendor/iiiaka/
*      https://www.patreon.com/iiiaka
*      https://boosty.to/iiiaka
*  GitHub repository page: https://github.com/IIIaKa/FreeRT
*  
*  uMod plugin page: https://umod.org/plugins/free-rt
*  uMod license: https://umod.org/plugins/free-rt#license
*  
*  Codefling plugin page: https://codefling.com/plugins/free-rt
*  Codefling license: https://codefling.com/plugins/free-rt?tab=downloads_field_4
*  
*  Lone.Design plugin page: https://lone.design/product/free-rt/
*
*  Copyright © 2020-2026 IIIaKa
*/

using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Oxide.Core;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
	[Info("Free RT", "IIIaKa", "0.1.10")]
	[Description("A simple plugin that allows players with permissions to open card-locked doors in Rad Towns without a card.")]
	class FreeRT : RustPlugin
	{
		[PluginReference]
        private Plugin  NCP, UINotify;

        #region ~Variables~
		private bool _ncpIsLoaded = false, _uiNotifyIsLoaded = false;
        private const string PERMISSION_ALL = "freert.all", PERMISSION_GREEN = "freert.green", PERMISSION_BLUE = "freert.blue", PERMISSION_RED = "freert.red";
		#endregion
		
		#region ~Configuration~
		private static Configuration _config;

		private class Configuration
		{
			[JsonProperty(PropertyName = "Is it worth showing messages to players who don't have permissions?")]
			public bool ShowMessage = true;
			
			[JsonProperty(PropertyName = "Is it worth enabling GameTips for messages?")]
            public bool GameTips_Enabled = true;
			
			[JsonProperty(PropertyName = "Is it worth using Notify plugins for messages instead of the vanilla UI?")]
            public bool Notify_Enabled = true;

            [JsonProperty(PropertyName = "Specify the message type for notify")]
            public int Notify_Type = 1;
			
			[JsonProperty(PropertyName = "Time in seconds(1-10) after which the door will close(hinged doors only)")]
            public float CloseTime = 5f;
			
			public Oxide.Core.VersionNumber Version;
		}
		
		protected override void LoadConfig()
        {
            base.LoadConfig();
            try { _config = Config.ReadObject<Configuration>(); }
            catch (Exception ex) { PrintError($"{ex.Message}\n\n[{Title}] Your configuration file contains an error."); }
            if (_config == null || _config.Version == new VersionNumber())
            {
                PrintWarning("The configuration file is not found or contains errors. Creating a new one...");
                LoadDefaultConfig();
            }
            else if (_config.Version < Version)
            {
				PrintWarning($"Your configuration file version({_config.Version}) is outdated. Updating it to {Version}...");
				string cfgPath = $"{Interface.Oxide.ConfigDirectory}{Path.DirectorySeparatorChar}{Name}.json";
                if (File.Exists(cfgPath))
                    File.Move(cfgPath, $"{Interface.Oxide.ConfigDirectory}{Path.DirectorySeparatorChar}_old_{Name}({_config.Version}).json");
				_config.Version = Version;
				PrintWarning($"The configuration file has been successfully updated to version {_config.Version}!");
            }
			
			_config.CloseTime = Mathf.Clamp(_config.CloseTime, 1f, 10f);
			
			SaveConfig();
        }
		
		protected override void SaveConfig() => Config.WriteObject(_config);
		protected override void LoadDefaultConfig() => _config = new Configuration() { Version = Version };
		#endregion

		#region ~Language~
		protected override void LoadDefaultMessages()
		{
			lang.RegisterMessages(new Dictionary<string, string>
			{
				["MsgNotAllowed"] = "You do not have permission to open this door without the card!"
			}, this);
			lang.RegisterMessages(new Dictionary<string, string>
			{
				["MsgNotAllowed"] = "У вас недостаточно прав для открытия этой двери без карточки!"
			}, this, "ru");
		}
        #endregion

        #region ~Methods~
		private void TryOpenDoor(CardReader cardReader, BasePlayer player, Door door = null)
        {
			bool canOpen = permission.UserHasPermission(player.UserIDString, PERMISSION_ALL);
			if (!canOpen)
			{
                switch (cardReader.accessLevel)
                {
                    case 1:
						canOpen = permission.UserHasPermission(player.UserIDString, PERMISSION_GREEN);
						break;
                    case 2:
						canOpen = permission.UserHasPermission(player.UserIDString, PERMISSION_BLUE);
						break;
                    case 3:
						canOpen = permission.UserHasPermission(player.UserIDString, PERMISSION_RED);
						break;
                    default:
                        break;
                }
            }
			if (!canOpen)
			{
				if (!_config.ShowMessage)
					return;
				
				string text = lang.GetMessage("MsgNotAllowed", this, player.UserIDString);
				if (_config.Notify_Enabled)
                {
					if (_ncpIsLoaded)
                    {
                        NCP.Call("SendNotify", player, _config.Notify_Type, text);
                        return;
                    }
                    if (_uiNotifyIsLoaded)
                    {
                        UINotify.Call("SendNotify", player, _config.Notify_Type, text);
                        return;
                    }
                }
				
				if (_config.GameTips_Enabled)
                    player.Command("gametip.showtoast", (int)GameTip.Styles.Error, text, string.Empty);
                else
                    player.ChatMessage(text);
				return;
			}
			
			if (door == null)
			{
				cardReader.GrantCard();
				return;
			}
			
			door.SetFlagLocal(BaseEntity.Flags.Open, true);
			timer.Once(_config.CloseTime, () =>
            {
                if (door != null && (cardReader == null || !cardReader.HasFlag(BaseEntity.Flags.On)))
					door.SetFlagLocal(BaseEntity.Flags.Open, false);
			});
		}
		#endregion

		#region ~Oxide Hooks~
		void OnDoorKnocked(Door door, BasePlayer player)
		{
			if (!door.isSecurityDoor || door.IsOpen())
				return;
			using PooledList<CardReader> crList = Facepunch.Pool.Get<PooledList<CardReader>>();
			Vis.Entities(door.transform.position, 6f, crList);
            if (crList.Count > 0)
                TryOpenDoor(crList[0], player, door);
		}
		
		void OnSwitchToggled(ElectricSwitch electricSwitch, BasePlayer player)
        {
			using PooledList<CardReader> crList = Facepunch.Pool.Get<PooledList<CardReader>>();
			Vis.Entities(electricSwitch.transform.position, 2f, crList);
            if (crList.Count > 0)
                TryOpenDoor(crList[0], player);
		}
		
		void OnButtonPress(PressButton button, BasePlayer player)
        {
			CardReader cardReader;
			foreach (var input in button.inputs)
            {
				cardReader = input.connectedTo?.ioEnt as CardReader;
				if (cardReader != null)
				{
					TryOpenDoor(cardReader, player);
					break;
				}
			}
		}
		
		void OnPluginLoaded(Plugin plugin)
        {
			if (plugin == NCP)
                _ncpIsLoaded = NCP != null && NCP.IsLoaded;
            else if (plugin == UINotify)
                _uiNotifyIsLoaded = UINotify != null && UINotify.IsLoaded;
        }
		
		void OnPluginUnloaded(Plugin plugin)
        {
			if (plugin.Name == "NCP")
                _ncpIsLoaded = false;
            else if (plugin.Name == "UINotify")
                _uiNotifyIsLoaded = false;
		}
		
		void Init()
        {
			Unsubscribe(nameof(OnPluginLoaded));
			Unsubscribe(nameof(OnPluginUnloaded));
			Unsubscribe(nameof(OnDoorKnocked));
			Unsubscribe(nameof(OnSwitchToggled));
			Unsubscribe(nameof(OnButtonPress));
			permission.RegisterPermission(PERMISSION_ALL, this);
            permission.RegisterPermission(PERMISSION_GREEN, this);
            permission.RegisterPermission(PERMISSION_BLUE, this);
            permission.RegisterPermission(PERMISSION_RED, this);
		}
		
		void OnServerInitialized(bool initial)
        {
			_ncpIsLoaded = NCP != null && NCP.IsLoaded;
            _uiNotifyIsLoaded = UINotify != null && UINotify.IsLoaded;
            Subscribe(nameof(OnDoorKnocked));
			Subscribe(nameof(OnSwitchToggled));
			Subscribe(nameof(OnButtonPress));
			Subscribe(nameof(OnPluginLoaded));
			Subscribe(nameof(OnPluginUnloaded));
			if (_config.Notify_Enabled && !_ncpIsLoaded && !_uiNotifyIsLoaded)
                PrintWarning("You have Notify plugin support enabled, but none were found!\n* https://codefling.com/plugins/ncp\n* https://umod.org/plugins/ui-notify");
		}
		
		void Unload() => _config = null;
		#endregion
	}
}