using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Julas.Utils;
using Ozeki.VoIP;

namespace VoipClient
{
    public class VoipClientModule : IDisposable
    {
        private readonly Softphone _softphone;
        private readonly object _lockObj = new object();
        private readonly string _serverIp;
        private readonly int _serverPort;

        public PhoneState PhoneState { get; private set; }
        public event Action<PhoneState> PhoneStateChanged; 

        public VoipClientModule(string serverIp, int serverPort)
        {
            PhoneState = new PhoneState {Status = PhoneStatus.Unregistered, OtherUserId = null};
            _softphone = new Softphone();
            _serverIp = serverIp;
            _serverPort = serverPort;
            AttachEvents();
        }

        public void Register(string thisUserId, string sessionId)
        {
            lock (_lockObj)
            {
                if (PhoneState.Status != PhoneStatus.Unregistered) return;
                SetPhoneStatus(PhoneStatus.Registering, null);
                _softphone.Register(
                    true,
                    thisUserId,
                    thisUserId,
                    sessionId,
                    string.Empty,
                    _serverIp,
                    _serverPort);
            }
        }

        public void StartCall(string otherUserId)
        {
            lock (_lockObj)
            {
                if (PhoneState.Status != PhoneStatus.Registered) return;
                _softphone.StartCall($"{otherUserId}@{_serverIp}:{_serverPort}");
                SetPhoneStatus(PhoneStatus.Calling, otherUserId);
            }
        }

        public void EndCall()
        {
            lock (_lockObj)
            {
                if (!PhoneState.Status.IsOneOf(PhoneStatus.Calling, PhoneStatus.InCall, PhoneStatus.IncomingCall)) return;
                _softphone.HangUp();
                SetPhoneStatus(PhoneStatus.Registered, null);
            }
        }

        public void AnswerCall()
        {
            lock (_lockObj)
            {
                if (PhoneState.Status != PhoneStatus.IncomingCall) return;
                _softphone.AcceptCall();
                SetPhoneStatus(PhoneStatus.InCall, PhoneState.OtherUserId);
            }
        }

        public void RejectCall()
        {
            lock (_lockObj)
            {
                if (PhoneState.Status != PhoneStatus.IncomingCall) return;
                _softphone.HangUp();
                SetPhoneStatus(PhoneStatus.Registered, null);
            }
        }

        private void SetPhoneStatus(PhoneStatus status, string otherUserId)
        {
            if (PhoneState.Status != status || PhoneState.OtherUserId != otherUserId)
            {
                PhoneState = new PhoneState { OtherUserId = otherUserId, Status = status };
                PhoneStateChanged?.Invoke(PhoneState);
            }
        }

        private void AttachEvents()
        {
            _softphone.PhoneLineStateChanged += SoftphoneOnPhoneLineStateChanged;
            _softphone.CallStateChanged += SoftphoneOnCallStateChanged;
            _softphone.IncomingCall += SoftphoneOnIncomingCall;
        }

        private void SoftphoneOnIncomingCall(string callerId)
        {
            lock (_lockObj)
            {
                SetPhoneStatus(PhoneStatus.IncomingCall, callerId);
            }
        }

        private void SoftphoneOnCallStateChanged(object sender, CallStateChangedArgs e)
        {
            lock (_lockObj)
            {
                if (e.State == CallState.InCall || e.State == CallState.Answered)
                {
                    SetPhoneStatus(PhoneStatus.InCall, PhoneState.OtherUserId);
                }
                else if (e.State.IsOneOf(CallState.Forwarding, CallState.Queued, CallState.Ringing,
                    CallState.Transferring))
                {
                    if (PhoneState.Status != PhoneStatus.IncomingCall)
                    {
                        SetPhoneStatus(PhoneStatus.Calling, PhoneState.OtherUserId);
                    }
                }
                else
                {
                    SetPhoneStatus(PhoneStatus.Registered, null);
                }
            }
        }

        private void SoftphoneOnPhoneLineStateChanged(object sender, RegistrationStateChangedArgs e)
        {
            lock (_lockObj)
            {
                if(e.State.IsOneOf(RegState.Error, RegState.NotRegistered, RegState.UnregRequested))
                    SetPhoneStatus(PhoneStatus.Unregistered, null);
                else if (e.State.IsOneOf(RegState.RegistrationRequested))
                {
                    SetPhoneStatus(PhoneStatus.Registering, null);
                }
                else
                {
                    SetPhoneStatus(PhoneStatus.Registered, null);
                }
            }
        }

        private void DeattachEvents()
        {
            _softphone.PhoneLineStateChanged -= SoftphoneOnPhoneLineStateChanged;
            _softphone.CallStateChanged -= SoftphoneOnCallStateChanged;
            _softphone.IncomingCall -= SoftphoneOnIncomingCall;
        }

        public void Dispose()
        {
            lock (_lockObj)
            {
                EndCall();
                DeattachEvents();
                _softphone.Dispose();
            }
        }
    }
}
