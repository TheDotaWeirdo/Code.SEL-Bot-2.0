using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Timers;

namespace Code.SEL_Bot_2._0
{
	public class Exile
	{
		public User User;
		public Role[] SaveRoles;
		public DateTime ExpireTime;
		public Timer Timer = new Timer(1000);
		public double Timeleft;
		private bool ReducedTime = false;
		private bool WasMuted;
		private bool WasDeafened;
		public MessageEventArgs e;

		public void AddTime(int Seconds)
		{
			ExpireTime.AddSeconds(Seconds);
			e.Channel.SendMessage(User.Mention + ", your **Exile** has been **increased** until " + ExpireTime.ToShortTimeString()).Timed(Timeleft);
			Timeleft += Seconds;
		}

		public void ReduceTime()
		{
			if (!ReducedTime)
				ExpireTime = new DateTime((ExpireTime.Ticks - DateTime.Now.Ticks) / 3);
			else
				(e.Channel.SendMessage(User.Mention + ", you can no longer reduce your exile time")).Timed(60);
			ReducedTime = true;
		}

		public void ExileUser()
		{
			SaveRoles = User.Roles.ToArray();
			WasMuted = User.IsServerMuted;
			WasDeafened = User.IsServerDeafened;
			User.Edit(true, true, null, new Role[] { e.Server.GetRole(Dictionaries.Role_Exiled) });
			e.Channel.SendMessage(User.Mention + ", you've been **Exiled** until " + ExpireTime.ToShortTimeString()).Timed(Timeleft);
			Timer.Elapsed += (s, w) =>
			{
				Timeleft--;
				if (Timeleft == 0)
					Timer.Dispose();
			};
		}

		public void UnexileUser()
		{
			User.Edit(WasMuted, WasDeafened, null, SaveRoles);
			e.Channel.SendMessage(User.Mention + ", you've been **Unexiled**").Timed(30);
		}
	}
}
