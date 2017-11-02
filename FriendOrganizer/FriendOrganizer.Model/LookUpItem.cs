namespace FriendOrganizer.Model
{
    public class LookUpItem
    {
        public int Id { get; set; }

        public string DisplayMember { get; set; }

    }

    public class NullLookupItem : LookUpItem
    {
        public new int? Id { get { return null;  } }
    }
}
