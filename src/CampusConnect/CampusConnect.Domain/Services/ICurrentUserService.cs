using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusConnect.Domain.Services;
public interface ICurrentUserService
{
    int? GetCurrentUserId();
    string GetCurrentUserRole();
    bool IsInRole(string roleName);
}