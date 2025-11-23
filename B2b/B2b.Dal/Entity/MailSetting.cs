using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class MailSetting
    {
        [Key]
        public int MailSettingId { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }
}
