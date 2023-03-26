using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal interface IColumn
    {
        int Id { get; }
        int Board { get; set; }
        string Title { get; set; }
        int Ordinal { get; set; }
        int Limit { get; set; }
    }
}
