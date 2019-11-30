using System;

namespace CustomerService.Models
{
    /// <summary>
    /// Customer Details
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets or sets the the Customer Id
        /// </summary>
        /// <value>
        /// Unique Customer Id
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the the Customer First Name
        /// </summary>
        /// /// <value>
        /// Customer First Name
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the the Customer Last Name
        /// </summary>
        /// /// <value>
        /// Customer Last Name
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the the Customer Date of Birth
        /// </summary>
        /// /// <value>
        /// Customer Date of Birth
        /// </value>
        public DateTime DateOfBirth { get; set; }
    }
}
