using System;

namespace CustomerApi.VM
{
    public class CustomerVm
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCustomerVm
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class UpdateCustomerVm
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
