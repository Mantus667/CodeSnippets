/*
Helper to send sms with Twilio
*/
using System;
using System.Configuration;
using Twilio;
using Twilio.Types;
using Umbraco.Core.Logging;

namespace TwilioHelper
{
    public static class TwilioHelper
    {
        static string _accountSid = ConfigurationManager.AppSettings["TwilioAccountSID"].ToString();
        static string _authToken = ConfigurationManager.AppSettings["TwilioAuthToken"].ToString();
        static string _messageServiceSid = ConfigurationManager.AppSettings["TwilioMessageServiceSID"].ToString();
        static string _smsSenderId = ConfigurationManager.AppSettings["TwilioSmsSenderId"].ToString();
        //static string _twilioUkNumber = ConfigurationManager.AppSettings["TwilioUKNumber"].ToString();
        //static string _twilioUsNumber = ConfigurationManager.AppSettings["TwilioUSNumber"].ToString();

        public static string GetFormattedPhoneNumber(string phoneNumber, string defaultPrefix = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }

            if (phoneNumber.StartsWith("+"))
            {
                return phoneNumber
                    .Replace(" ", string.Empty);
            }

            return "+" + defaultPrefix + phoneNumber
                .Replace(" ", string.Empty)
                .TrimStart('0');
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);
                var result = Twilio.Rest.Lookups.V1.PhoneNumberResource.Fetch(new Twilio.Rest.Lookups.V1.FetchPhoneNumberOptions(phoneNumber));
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Warn(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Failed to validate phone number: '{phoneNumber}.\n" + ex.Message);
                return false;
            }
        }

        public static bool SendBySms(string phoneNumber, string body, string name, string email)
        {
            if (string.IsNullOrWhiteSpace(_smsSenderId) || _smsSenderId.Length > 11)
            {
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Invalid SMS sender ID supplied", new Exception("Invalid SMS sender ID supplied"));
                return false;
            }
            try
            {
                TwilioClient.Init(_accountSid, _authToken);

                var message = Twilio.Rest.Api.V2010.Account.MessageResource.Create(
                    new PhoneNumber(phoneNumber),
                    messagingServiceSid: _messageServiceSid,
                    body: body,
                    from: _smsSenderId
                    );

                if (!message.ErrorCode.HasValue)
                {
                    LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"PIN sent to user {name} ({email}) on {phoneNumber}) with Sid {message.Sid}");
                    return true;
                }
                else
                {
                    LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, $"Failed to send PIN to user {name} ({email}) on {phoneNumber}) with error: {message.ErrorMessage}", new Exception(message.ErrorMessage));
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Failed to send SMS", ex);
                return false;
            }
        }

        //public void SendByCall(string landline, string pin, string name, string email)
        //{
        //    var baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host;

        //    try
        //    {
        //        var fromNumber = landline.StartsWith("1") ? _twilioUsNumber : _twilioUkNumber;
        //        TwilioClient.Init(_accountSid, _authToken);

        //        var message = Twilio.Rest.Api.V2010.Account.CallResource.Create(
        //            new PhoneNumber(landline),
        //            from: new PhoneNumber(fromNumber),
        //            url: new Uri(baseUrl + "/pin/voicepin?pin=" + pin)
        //            );
        //        LogHelper.Info(this.GetType(), string.Format("Called user for PIN {0} ({1}) on {2}) with Sid {3}", name, email, landline, message.Sid));
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Failed to send SMS", ex);
        //    }
        //}
    }
}