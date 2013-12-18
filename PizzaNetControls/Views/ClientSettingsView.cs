using PizzaNetControls.Configuration;
using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.DTOs;
using PizzaNetWorkClient.WCFClientInfrastructure;
using PizzaNetCommon.Requests;
using PizzaNetControls.Common;

namespace PizzaNetControls.Views
{
    public class ClientSettingsView : BaseView
    {
        public ClientSettingsView(IWorker worker)
            : base(worker)
        {
            ModifiedPasssword = ModifiedUserData = false;
        }

        public bool ModifiedPasssword { get; set; }
        public bool ModifiedUserData { get; set; }
        public bool Modified { get { return ModifiedPasssword || ModifiedUserData; } }

        private User user;
        public User User
        {
            get { return user; }
            set { user = value; NotifyPropertyChanged("User"); }
        }

        #region passwords
        private string _cpass;
        public string CurrentPassword
        {
            get { return _cpass; }
            set { _cpass = value; NotifyPropertyChanged("CurrentPassword"); }
        }

        private string _pass;
        public string Password
        {
            get { return _pass; }
            set { _pass = value; NotifyPropertyChanged("Password"); }
        }

        private string _passn;
        public string NewPassword
        {
            get { return _passn; }
            set { _passn = value; NotifyPropertyChanged("NewPassword"); }
        }

        private string _passr;
        public string PasswordRepeated
        {
            get { return _passr; }
            set { _passr = value; NotifyPropertyChanged("PasswordRepeated"); }
        }

        private bool _haserr;
        public bool HasValidationError
        {
            get { return _haserr; }
            set { _haserr = value; NotifyPropertyChanged("HasValidationError"); }
        }

        private bool _hascurrerr;
        public bool HasCurrentValidationError
        {
            get { return _hascurrerr; }
            set { _hascurrerr = value; NotifyPropertyChanged("HasCurrentValidationError"); }
        }
        #endregion

        public void Load()
        {
            ModifiedPasssword = ModifiedUserData = false;
            Worker.EnqueueTask(new WorkerTask((args) =>
            {
                var user = args[0] as User;
                try
                {
                    if (user == null)
                        throw new PizzaNetException(Utils.Messages.NO_USER_LOGGED_IN);
                    using (var proxy = new WorkChannel())
                    {
                        return proxy.GetUser(new EmptyRequest { Login = user.Email, Password = user.Password });
                    }
                }
                catch (Exception e)
                {
                    return e;
                }
            }, (s, args) =>
            {
                if (args.Result is Exception)
                {
                    Utils.HandleException(args.Result as Exception);
                    return;
                }
                var u = args.Result as SingleItemResponse<UserDTO>;
                if (u == null)
                {
                    Utils.showError(String.Format(Utils.Messages.NO_USER_FOUND_FORMAT, ClientConfig.CurrentUser.Email));
                    return;
                }
                ClientConfig.CurrentUser.UpdateWithUserDTO(u.Data);
                this.User = ClientConfig.CurrentUser;
                CurrentPassword = PasswordRepeated = NewPassword = "";
                Password = ClientConfig.CurrentUser.Password;
            }, ClientConfig.CurrentUser));
        }

        internal void SaveUserInfo()
        {
            Worker.EnqueueTask(new WorkerTask(args =>
                {
                    try
                    {
                        using (var proxy = new WorkChannel())
                        {
                            var data = new UserDTO
                            {
                                UserID = User.UserID,
                                Address = User.Address,
                                Email = User.Email,
                                Name = User.Name,
                                Password = null,
                                Phone = User.Phone,
                                Rights = User.Rights
                            };
                            return proxy.UpdateUser(new UpdateRequest<UserDTO>()
                            {
                                Login = User.Email,
                                Password = User.Password,
                                Data = data
                            });
                        }
                    }
                    catch (Exception exc)
                    {
                        return exc;
                    }
                }, (s, e) =>
                {
                    if (e.Result is Exception)
                    {
                        Utils.HandleException(e.Result as Exception);
                        return;
                    }
                    var userRes = e.Result as SingleItemResponse<UserDTO>;
                    if (userRes == null)
                    {
                        Utils.showError(Utils.Messages.UNKNOWN_ERROR_FORMAT);
                        return;
                    }
                    ClientConfig.CurrentUser.UpdateWithUserDTO(userRes.Data);
                    ClientConfig.CurrentUser.RefreshRate = this.User.RefreshRate;
                    this.User = ClientConfig.CurrentUser;
                    ModifiedUserData = false;
                    Utils.showInformation(Utils.Messages.SAVED_SUCCESSFULLY);
                }, null));
        }

        internal void SavePassword()
        {
            if (Password != CurrentPassword)
            {
                Utils.showExclamation(Utils.Messages.INVALID_PASSWORD);
                return;
            }
            if (NewPassword != PasswordRepeated)
            {
                Utils.showExclamation(Utils.Messages.REPEATED_PASSWORD_DIFF);
                return;
            }
            if (Password == string.Empty || Password == null ||
                NewPassword == string.Empty || NewPassword == null)
            {
                Utils.showExclamation(Utils.Messages.PASSWORD_EMPTY);
                return;
            }
            Worker.EnqueueTask(new WorkerTask(args =>
            {
                var user = args[0] as User;
                var newPass = args[1] as string;
                try
                {
                    if (user == null)
                        throw new PizzaNetException(Utils.Messages.NO_USER_LOGGED_IN);
                    using (var proxy = new WorkChannel())
                    {
                        return proxy.UpdateUser(new UpdateRequest<UserDTO>()
                            {
                                Login = user.Email,
                                Password = user.Password,
                                Data = new UserDTO()
                                {
                                    UserID = user.UserID,
                                    Address = null,
                                    Email = user.Email,
                                    Name = null,
                                    Password = newPass,
                                    Phone = -1,
                                }
                            });
                    }
                }
                catch (Exception e)
                {
                    return e;
                }
            },
                (s, e) =>
                {
                    if (e.Result is Exception)
                    {
                        Utils.HandleException(e.Result as Exception);
                        return;
                    }
                    var userRes = e.Result as SingleItemResponse<UserDTO>;
                    if (userRes == null || userRes.Data == null)
                    {
                        Utils.showError(Utils.Messages.UNKNOWN_ERROR_FORMAT);
                        return;
                    }
                    User.Password = userRes.Data.Password;
                    Password = User.Password;
                    CurrentPassword = PasswordRepeated = NewPassword = "";
                    ModifiedPasssword = false;
                    Utils.showInformation(Utils.Messages.PASSWORD_CHANGED);
                }, ClientConfig.CurrentUser, NewPassword));
        }
    }
}
