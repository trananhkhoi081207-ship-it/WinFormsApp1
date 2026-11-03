namespace LabMaintenanceApp
{
    public static class RolePermissions
    {
        public static bool CanManageUsers(string role) => role == "Admin";
        public static bool CanManageEquipment(string role) => role == "Admin" || role == "LabManager";
        public static bool CanManageLabs(string role) => CanManageEquipment(role);
        public static bool CanAssignTechnician(string role) => CanManageEquipment(role);
        public static bool CanCreateRequest(string role) => CanAssignTechnician(role) || role == "Technician";
        public static bool CanCompleteMaintenance(string role) => role == "Admin" || role == "Technician";
    }
}
