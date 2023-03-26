using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal interface ITask
    {
        int Id { get; }
        DateTime Created { get; }
        string Assignee { get; set; }
        DateTime Due { get; set; }
        string Title { get; set; }
        string Description { get; set; }

        void Remove();
        void MoveColumn(int id);
        ServiceLayer.Task ToServiceTask();
    }
}
