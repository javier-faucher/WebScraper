using Scraper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.Data_Layer
{
    public class SessionRepo
    {
        private readonly ScraperContext ctx;

        public SessionRepo(ScraperContext _ctx)
        {
            ctx = _ctx;
        }

        public bool saveSession(Session session)
        {
            try
            {
                ctx.sessions.Add(session);
                ctx.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<Session> getAllSessions()
        {
            List<Session> sessions = new List<Session>();

            foreach (Session session in ctx.sessions)
            {
                sessions.Add(session);
            }
            return sessions;
        }

    }
}

