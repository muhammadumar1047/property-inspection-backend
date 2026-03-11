using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Entities
{
    public class NotificationRecipient : BaseEntity
    {
        public Guid? NotificationId { get; set; }
        public Guid? AgencyId { get; set; }
        public Guid UserId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadDate { get; set; }
        public Notification? Notification { get; set; }
    }
}
