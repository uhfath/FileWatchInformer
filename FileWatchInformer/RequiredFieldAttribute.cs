using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatchInformer
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    internal sealed class RequiredFieldAttribute : RequiredAttribute
    {
        public RequiredFieldAttribute(string resourceName)
        {
            AllowEmptyStrings = false;
            ErrorMessageResourceType = typeof(Messages);
            ErrorMessageResourceName = resourceName;
        }
    }
}
