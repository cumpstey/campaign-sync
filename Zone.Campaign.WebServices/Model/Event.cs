using System.Xml;

namespace Zone.Campaign.WebServices.Model
{
    /// <summary>
    /// Class representing a real-time event (nms:rtEvent) for triggering an email in Message Center.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Event type. Should correspond to a value in the eventType enumeration.
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Email address of the recipient.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Origin of the event.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Channel on which the delivery should be trigegred.
        /// </summary>
        public DeliveryType WishedChannel { get; set; }

        /// <summary>
        /// An external id, for reference only.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Document containing the ctx data of the event. The root node must be named "ctx".
        /// </summary>
        public XmlDocument ContextData { get; set; }
    }
}
