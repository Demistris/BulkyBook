using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public static class SD
    {
        public const string ROLE_USER_INDIVIDUAL = "Individual";
        public const string ROLE_USER_COMPANY = "Company";
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_EMPLOYEE = "Employee";

        public const string STATUS_PENDING = "Pending";
        public const string STATUS_APPROVED = "Approved";
        public const string STATUS_IN_PROGERSS = "Proccessing";
        public const string STATUS_SHIPPED = "Shipped";
        public const string STATUS_CANCELLED = "Cancelled";
        public const string STATUS_REFUNDED = "Refunded";

        public const string PAYMENT_STATUS_PENDING = "Pending";
		public const string PAYMENT_STATUS_APPROVED = "Approved";
		public const string PAYMENT_STATUS_DELEYED_PAYMENT = "ApprovedForDeleyedPayment";
		public const string PAYMENT_STATUS_REJECTED = "Rejected";
	}
}
