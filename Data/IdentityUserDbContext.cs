using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalrChatApp.Models;

namespace SignalrChatApp.Data;

public class IdentityUserDbContext : IdentityDbContext<AppUser>
{
    public IdentityUserDbContext(DbContextOptions<IdentityUserDbContext> options) : base(options)
    {

    }
}
