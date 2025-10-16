using System;

namespace GyeMong.UISystem.Social.Dto
{
    [Serializable]
    public class JoinRequest
    {
        public string email;
        public string name;
        public string password;
    }
}