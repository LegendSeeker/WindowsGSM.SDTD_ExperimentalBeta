using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System;
using WindowsGSM.Installer;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
    /// <summary>
    /// 
    /// Notes:
    /// 7 Days to Die Dedicated Server has a special console template which, when RedirectStandardOutput=true, the console output is still working.
    /// The console output seems have 3 channels, RedirectStandardOutput catch the first channel, RedirectStandardError catch the second channel, the third channel left on the game server console.
    /// Moreover, it has his input bar on the bottom so a normal sendkey method is not working.
    /// We need to send a {TAB} => (Send text) => {TAB} => (Send text) => {ENTER} to make the input cursor is on the input bar and send the command successfully.
    /// 
    /// RedirectStandardInput:  NOT WORKING
    /// RedirectStandardOutput: YES (Used)
    /// RedirectStandardError:  YES (Used)
    /// SendKeys Input Method:  YES (Used)
    /// 
    /// There are two methods to shutdown this special server
    /// 1. {TAB} => (Send shutdown) => {TAB} => (Send shutdown) => {ENTER}
    /// 2. p.CloseMainWindow(); => {ENTER}
    /// 
    /// The second one is used.
    /// 
    /// </summary>
    public class SDTD_ExperimentalBeta : SteamCMDAgent
    {
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.SDTD_ExperimentalBeta",
            author = "LegendSeeker",
            description = "🧩 WindowsGSM plugin for supporting 7 Days to Die Dedicated Server running Experimental Beta",
            version = "0.1",
            url = "https://github.com/LegendSeeker/WindowsGSM.SDTD_ExperimentalBeta",
            color = "#5c1504" // Color Hex
        };

        private readonly Functions.ServerConfig _serverData;

        public new string Error;
        public new string Notice;

        public string FullName = "7DTD Dedicated Server - Experimental Beta";
        public new string StartPath = "7DaysToDieServer.exe";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 1;
        public dynamic QueryMethod = new A2S();

        public string Port = "26900";
        public string QueryPort = "26900";
        public string Defaultmap = "Navezgane";
        public string Maxplayers = "8";
        public string Additional = string.Empty;

        public new string AppId = "294420";

        public SDTD_ExperimentalBeta(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        public string OldProfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\7DaysToDie\\Saves");

        public async void CreateServerCFG()
        {
            //Download serverconfig.xml
            string configPath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, "serverconfig.xml");
            if (await Functions.Github.DownloadGameServerConfig(configPath, FullName))
            {
                if (File.Exists(OldProfile))
                    await UI.CreateYesNoPromptV1("Old server userdata found", $"An Old Savegame found in {OldProfile}\n\n" +
                        $"If you want to use it, you need to move/copy it manually to \n" +
                        $"servers/{_serverData.ServerID}/serverfiles/userdata", "Ok", "Ok");
                string configText = File.ReadAllText(configPath);
                configText = configText.Replace("{{hostname}}", _serverData.ServerName);
                configText = configText.Replace("{{rcon_password}}", _serverData.GetRCONPassword());
                configText = configText.Replace("{{port}}", _serverData.ServerPort);
                configText = configText.Replace("{{telnetPort}}", (int.Parse(_serverData.ServerPort) - int.Parse(Port) + 8081).ToString());
                configText = configText.Replace("{{maxplayers}}", Maxplayers);
                File.WriteAllText(configPath, configText);
            }

            //Create steam_appid.txt
            string txtPath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, "steam_appid.txt");
            File.WriteAllText(txtPath, "251570");
        }

        public async Task<Process> Start()
        {
            string exeName = "7DaysToDieServer.exe";
            string workingDir = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID);
            string exePath = Path.Combine(workingDir, exeName);

            if (!File.Exists(exePath))
            {
                Error = $"{exeName} not found ({exePath})";
                return null;
            }

            string configPath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, "serverconfig.xml");
            if (!File.Exists(configPath))
            {
                Notice = $"serverconfig.xml not found ({configPath})";
            }

            if (File.Exists(OldProfile))
            {
                Notice = $"An Old Savegame found in {OldProfile}\n\n" +
                        $"If you want to use it, you need to move/copy it manually to \n" +
                        $"servers/{_serverData.ServerID}/serverfiles/userdata.\n" +
                        $"If this is intentional, ignore this";
            }

            string logFile = @"7DaysToDieServer_Data\output_log_dedi__" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
            string param = $"-logfile \"{Path.Combine(workingDir, logFile)}\" -quit -batchmode -nographics -configfile=serverconfig.xml -dedicated {_serverData.ServerParam}";

            Process p;
            if (!AllowsEmbedConsole)
            {
                p = new Process
                {
                    StartInfo =
                    {
                        WorkingDirectory = workingDir,
                        FileName = exePath,
                        Arguments = param,
                        WindowStyle = ProcessWindowStyle.Minimized
                    },
                    EnableRaisingEvents = true
                };
                p.Start();
            }
            else
            {
                p = new Process
                {
                    StartInfo =
                    {
                        WorkingDirectory = workingDir,
                        FileName = exePath,
                        Arguments = param,
                        WindowStyle = ProcessWindowStyle.Minimized,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    },
                    EnableRaisingEvents = true
                };
                var serverConsole = new Functions.ServerConsole(_serverData.ServerID);
                p.OutputDataReceived += serverConsole.AddOutput;
                p.ErrorDataReceived += serverConsole.AddOutput;
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
            }

            return p;
        }

        public async Task Stop(Process p)
        {
            await Task.Run(async () =>
            {
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SendWaitToMainWindow("^c");
                Thread.Sleep(500);
                Functions.ServerConsole.SendWaitToMainWindow("{ENTER}");

                p.CloseMainWindow();
                Thread.Sleep(1000);
                if (!p.HasExited)
                    p.Kill();
            });
        }

        public new async Task<Process> Update(bool validate = false, string custom = null)
        {
            custom = "-beta latest_experimental";
            validate = true;

            var (p, error) = await Installer.SteamCMD.UpdateEx(_serverData.ServerID, AppId, validate, custom: custom);
            Error = error;
            return p;
        }

        public new bool IsInstallValid()
        {
            string exeFile = "7DaysToDieServer.exe";
            string exePath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, exeFile);

            return File.Exists(exePath);
        }

        public new bool IsImportValid(string path)
        {
            string exeFile = "7DaysToDieServer.exe";
            string exePath = Path.Combine(path, exeFile);

            Error = $"Invalid Path! Fail to find {exeFile}";
            return File.Exists(exePath);
        }

        public new string GetLocalBuild()
        {
            var steamCMD = new Installer.SteamCMD();
            return steamCMD.GetLocalBuild(_serverData.ServerID, AppId);
        }

        public new async Task<string> GetRemoteBuild()
        {
            var steamCMD = new Installer.SteamCMD();
            return await steamCMD.GetRemoteBuild(AppId);
        }
    }
}