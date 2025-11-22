namespace TestApp.VModels
{
    internal class StaffWithRoleViewModel
    {
        public int StaffID { get; set; }
        public string StaffName { get; set; }
        public string DOB { get; set; }
        public string DOJ { get; set; }
        public string Status { get; set; }
        public string RolePermission { get; set; }
        public bool HasLogin { get; set; }
        public string ContactNum { get; set; }
    }
}