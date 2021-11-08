using System.Collections.Generic;
using System.Linq;
using MediaStreamer.Domain;

namespace MediaStreamer.WindowsDesktop
{
    /// <summary>
    /// Логика взаимодействия для GroupMembersPage.xaml
    /// </summary>
    public partial class GroupMembersPage : FirstFMPage
    {
        //static List<GroupMember> groupMembers;
        public bool lastDataLoadWasPartial = false;
        public List<GroupMember> GroupMembers { get; set; }
        public List<GroupMember> GetGroupMembers()
        {
            Program.DBAccess.Update();
            return Program.DBAccess.DB.GetGroupMembers().ToList();
        }
        public void ListGroupMembers()
        {
            GroupMembers = GetGroupMembers();
        }
        public GroupMembersPage()
        {
            GroupMembers = new List<GroupMember>();
            InitializeComponent();
            ListGroupMembers();
            DataContext = this;
        }

    }
}
