namespace Raygun.Messages
{
    using System;

    public class RaygunMessage
    {
        public RaygunMessage()
        {
            OccurredOn = DateTime.UtcNow;
            Details = new RaygunMessageDetails();
        }

        public DateTime OccurredOn { get; set; }

        public RaygunMessageDetails Details { get; set; }

        public override string ToString()
        {
            return string.Format("[RaygunMessage: OccurredOn={0}, Details={1}]", OccurredOn, Details);
        }
    }
}