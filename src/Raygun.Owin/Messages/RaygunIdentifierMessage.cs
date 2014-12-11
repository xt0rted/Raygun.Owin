namespace Raygun.Messages
{
    public class RaygunIdentifierMessage
    {
        public RaygunIdentifierMessage(string user)
        {
            Identifier = user;
        }

        public string Identifier { get; set; }

        public bool IsAnonymous { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string FirstName { get; set; }

        public string UUID { get; set; }

        public override string ToString()
        {
            return string.Format("[RaygunIdentifierMessage: Identifier={0}, IsAnonymous={1}, Email={2}, FullName={3}, FirstName={4}, UUID={5}]", Identifier, IsAnonymous, Email, FullName, FirstName, UUID);
        }
    }
}