using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWorkerService.Interfaces
{
    public interface IProcessReservationsService
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }
}
