using AppuntaMed.Application.Interfaces.Repositories;
using AppuntaMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppuntaMed.Persistence.Repositories
{
	public class ConsultingRoomRepository : Repository<ConsultingRoom>, IConsultingRoomRepository
	{
		public ConsultingRoomRepository(AppuntaMedDbContext context) : base(context)
		{
		}
	}
}
