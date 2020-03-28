using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ININ.IceLib.Connection;
using WebChatBuilderService.Icelib.Availability;
using WebChatBuilderService.Icelib.Configurations;
using WebChatBuilderService.Icelib.Interactions;
using WebChatBuilderService.Icelib.People;
using System.Threading;

namespace WebChatBuilderService.Icelib
{
    public class IcelibInitializer
    {
        private static Session _session;
        private static string _server;
        private static string _activeServer = "";
        private string _server1;
        private string _server2;
        private string _user;
        private string _password;
        private Logging _logging;
        private int _connectAttempts;
        private bool _isConnecting;

        private int _skillsRefreshTime;
        private int _workgroupsRefreshTime;
        private int _agentsRefreshTime;
        private int _schedulesRefreshTime;
        private int _licensingRefreshTime;
        private int _peopleRefreshTime;

        private WorkgroupConfigurations _workgroupConfigurations;
        private SkillConfigurations _skillConfigurations;
        private AgentConfigurations _agentConfigurations;
        private AgentLicenseConfigurations _agentLicenseConfigurations;
        private WorkgroupInteractions _workgroupInteractions;
        private AgentAvailability _agentAvailability;
        private ScheduleConfigurations _scheduleConfigurations;

        private WorkgroupPeople _workgroupPeople;

        public static Session IcelibSession
        {
            get { return _session; }
        }

        public static string ActiveServer
        {
            get{return _activeServer;}
        }

        public void Start()
        {
            _logging = Logging.GetInstance();
            _workgroupConfigurations = new WorkgroupConfigurations();
            _skillConfigurations = new SkillConfigurations();
            _agentConfigurations = new AgentConfigurations();
            _agentLicenseConfigurations = new AgentLicenseConfigurations();
            _scheduleConfigurations = new ScheduleConfigurations(); 
            _workgroupPeople = new WorkgroupPeople();
            _workgroupInteractions = new WorkgroupInteractions();
            _agentAvailability = new AgentAvailability();
            //use ipaddress.tryparse and use dns lookup if it fails
            _server1 = System.Configuration.ConfigurationManager.AppSettings["PrimaryServer"];
            _server2 = System.Configuration.ConfigurationManager.AppSettings["SecondaryServer"];
            _user = System.Configuration.ConfigurationManager.AppSettings["CicUser"];
            _password = System.Configuration.ConfigurationManager.AppSettings["CicPassword"];
            _connectAttempts = 0;
            Connect();
        }

        private void Connect()
        {
            _connectAttempts++;
            if (String.IsNullOrEmpty(_server))
            {
                _server = _server1;
            }
            _session = new Session();
            _session.ConnectionStateChanged += session_ConnectionStateChanged;

            if (_session.ConnectionState != ConnectionState.Up)
            {
                _logging.TraceMessage(0, "Qsect WebChatBuilder is trying to connect to: " + _server);
                try
                {
                    SessionSettings sessionSettings = new SessionSettings();
                    HostSettings hostSettings = new HostSettings();
                    hostSettings.HostEndpoint = new HostEndpoint(_server, HostEndpoint.DefaultPort);

                    StationSettings stationSettings = new StationlessSettings();
                    AuthSettings authSettings = new WindowsAuthSettings();
                    var userName = _user;
                    var password = _password;
                    if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
                    {
                        authSettings = new ICAuthSettings(userName, password);
                    }

                    _session.ConnectAsync(sessionSettings, hostSettings, authSettings, stationSettings, session_ConnectCompleted, null);
                }
                catch (Exception Ex)
                {
                    PartialUnload();
                    _logging.TraceException(Ex, "Error connecting to CIC");
                    _server = _server == _server1 ? _server2 : _server1;
                    TryToReconnect();
                }
            }
        }

        void TryToReconnect()
        {
            if (_session == null || _session.ConnectionState != ConnectionState.Up)
            {
                if (_connectAttempts >= 4)
                {
                    _connectAttempts = 0;
                    var timer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5));
                }
                else
                {
                    Connect();
                }
            }
        }

        private void OnTimerElapsed(object state)
        {
            Connect();
        }

        private void session_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            _logging.TraceMessage(0, "Connection State Changed. Reason: " + e.Reason + " State: " + e.State);
            try
            {
                if (e.Reason == ConnectionStateReason.Switchover)
                {
                    if (_session.ConnectionState == ConnectionState.Up)
                    {
                        _session.Disconnect();
                    }

                    PartialUnload();

                    TryToReconnect();
                }
            }
            catch (Exception Ex)
            {
                _logging.TraceException(Ex, "ConnectionStateChanged Error");
            };
        }

        private void session_ConnectCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null || _session.ConnectionState == ConnectionState.Down)
            {
                PartialUnload();

                if (e.Error != null)
                {
                    _logging.TraceMessage(1, "Error connecting to CIC: " + e.Error.Message);
                }
                _server = _server == _server1 ? _server2 : _server1;
                TryToReconnect();
            }
            else
            {
                _connectAttempts = 0;
                _activeServer = _session.Endpoint.Host;
                _logging.TraceMessage(3, "Active Server set to: " + _activeServer);
                FinishLoading();
            }
        }

        public void PartialUnload()
        {
            _configRunning = false;
            _workgroupPeopleRunning = false;

            if (_skillConfigurations != null)
            {
                _skillConfigurations.Unload();
            }
            if (_workgroupConfigurations != null)
            {
                _workgroupConfigurations.Unload();
            }
            if (_agentConfigurations != null)
            {
                _agentConfigurations.Unload();
            }
            if (_agentLicenseConfigurations != null)
            {
                _agentLicenseConfigurations.Unload();
            }
            if (_scheduleConfigurations != null)
            {
                _scheduleConfigurations.Unload();
            }
            if (_workgroupPeople != null)
            {
                _workgroupPeople.Unload();
            }
            if (_workgroupInteractions != null)
            {
                _workgroupInteractions.Unload();
            }
            if (_agentAvailability != null)
            {
                _agentAvailability.Unload();
            }

            if (_session != null)
            {
                _session.ConnectionStateChanged -= session_ConnectionStateChanged;
            }
            _session = null;
        }


        private int ParseTime(string time, int min, int def)
        {
            int t;
            if (!String.IsNullOrWhiteSpace(time) && Int32.TryParse(time, out t))
            {
                if (t < min)
                {
                    return min;
                }
                return t;
            }
            return def;
        }

        private void LoadRefreshTimes()
        {
            var agentsRefreshTime = System.Configuration.ConfigurationManager.AppSettings["AgentsRefreshTime"]; //affects new users added
            var workgroupsRefreshTime = System.Configuration.ConfigurationManager.AppSettings["WorkgroupsRefreshTime"]; //affects new workgroups added
            var skillsRefreshTime = System.Configuration.ConfigurationManager.AppSettings["SkillsRefreshTime"]; //affects new skills added
            var schedulesRefreshTime = System.Configuration.ConfigurationManager.AppSettings["SchedulesRefreshTime"]; //affects new schedules added
            var licensingRefreshTime = System.Configuration.ConfigurationManager.AppSettings["LicensingRefreshTime"]; //affects licensing changes
            var peopleRefreshTime = System.Configuration.ConfigurationManager.AppSettings["PeopleRefreshTime"]; //affects workgroup membership and activation changes

            _agentsRefreshTime = ParseTime(agentsRefreshTime, 5, 60) * 1000;
            _workgroupsRefreshTime = ParseTime(workgroupsRefreshTime, 5, 60) * 1000;
            _skillsRefreshTime = ParseTime(skillsRefreshTime, 5, 60) * 1000;
            _schedulesRefreshTime = ParseTime(schedulesRefreshTime, 5, 60) * 1000;
            _licensingRefreshTime = ParseTime(licensingRefreshTime, 30, 60) * 1000;
            _peopleRefreshTime = ParseTime(peopleRefreshTime, 10, 30) * 1000;
        }

        private bool _configRunning = false;
        private bool _workgroupPeopleRunning = false;
        private bool _interactionsRunning = false;

        private void FinishLoading()
        {
            try
            {
                _skillConfigurations.Load(_session);
                _workgroupConfigurations.Load(_session);
                _agentConfigurations.Load(_session);
                _agentLicenseConfigurations.Load(_session);
                _scheduleConfigurations.Load(_session);
                _agentAvailability.Load(_session);
                _configRunning = true;
                
                _workgroupPeople.Load(_session);
                _workgroupPeopleRunning = true;

                _workgroupInteractions.Load(_session);
                _interactionsRunning = true;

                LoadRefreshTimes();

                var taskList = new List<Task> { RefreshSkills(), RefreshWorkgroups(), RefreshAgents(), RefreshSchedules(), RefreshAgentLicenseConfigurations(), RefreshPeople(), RefreshInteractions() };
                Task.WaitAll(taskList.ToArray());
            }
            catch (Exception e)
            {
            }
        }

        public async Task RefreshWorkgroups()
        {
            while (_configRunning)
            {
                _workgroupConfigurations.RefreshWatches();
                await Task.Delay(_workgroupsRefreshTime);
            }
        }

        public async Task RefreshAgents()
        {
            while (_configRunning)
            {
                _agentConfigurations.RefreshWatches();
                await Task.Delay(_agentsRefreshTime);
            }
        }

        public async Task RefreshSchedules()
        {
            while (_configRunning)
            {
                _scheduleConfigurations.RefreshWatches();
                await Task.Delay(_schedulesRefreshTime);
            }
        }

        public async Task RefreshSkills()
        {
            while (_configRunning)
            {
                _skillConfigurations.RefreshWatches();
                await Task.Delay(_skillsRefreshTime);
            }
        }

        public async Task RefreshAgentLicenseConfigurations()
        {
            while (_configRunning)
            {
                _agentLicenseConfigurations.RefreshWatches();
                await Task.Delay(_licensingRefreshTime);
            }
        }

        public async Task RefreshPeople()
        {
            while (_workgroupPeopleRunning)
            {
                _workgroupPeople.RefreshWatches();
                await Task.Delay(_peopleRefreshTime);
            }
        }

        public async Task RefreshInteractions()
        {
            while (_interactionsRunning)
            {
                _workgroupInteractions.RefreshWatches();
                await Task.Delay(5000);
            }
        }

        //private void ResolveDNSName(string host)
        //{
        //    try
        //    {
        //        var hostEntry = Dns.GetHostEntry(host);
        //        if (hostEntry.AddressList.Length > 0)
        //        {
        //            _server1 = hostEntry.AddressList[0].ToString();
        //        }
        //        if (hostEntry.AddressList.Length > 1)
        //        {
        //            _server2 = hostEntry.AddressList[1].ToString();
        //        }
        //    }
        //    catch (Exception Ex)
        //    {

        //    }
        //}

        public void Stop()
        {
            _configRunning = false;
            _workgroupPeopleRunning = false;
            _interactionsRunning = false;

            try
            {
                if (_skillConfigurations != null)
                {
                    _skillConfigurations.Unload();
                }
                if (_workgroupConfigurations != null)
                {
                    _workgroupConfigurations.Unload();
                }
                if (_agentConfigurations != null)
                {
                    _agentConfigurations.Unload();
                }
                if (_agentLicenseConfigurations != null)
                {
                    _agentLicenseConfigurations.Unload();
                }
                if (_scheduleConfigurations != null)
                {
                    _scheduleConfigurations.Unload();
                }
                if (_workgroupPeople != null)
                {
                    _workgroupPeople.Unload();                
                }
                if (_workgroupInteractions != null)
                {
                    _workgroupInteractions.Unload();
                }
                if (_agentAvailability != null)
                {
                    _agentAvailability.Unload();
                }
            }
            catch (Exception)
            {
            }

            if (_session != null)
            {
                _session.ConnectionStateChanged -= session_ConnectionStateChanged;

                try
                {
                    if (_session.ConnectionState == ConnectionState.Up)
                    {
                        _session.Disconnect();
                    }
                }
                catch (Exception)
                {
                }
            }
            _session = null;
        }
    }
}
