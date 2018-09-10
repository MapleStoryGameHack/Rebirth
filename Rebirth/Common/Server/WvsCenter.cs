﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Database;
using Common.Entities;
using Common.Provider;
using WvsRebirth;

namespace Common.Server
{
    public class WvsCenter : IDisposable
    {
        private readonly WvsLogin m_login;
        private readonly WvsGame[] m_games;

        public WzManager WzMan { get; }
        public MongoDb Db { get; }

        public WvsCenter(int channels)
        {
            WzMan = new WzManager();
            WzMan.LoadFile("Map.wz");

            Db = new MongoDb();

            m_login = new WvsLogin(this);
            m_games = new WvsGame[channels];

            for (int i = 0; i < m_games.Length; i++)
            {
                var channel = (byte) i;
                m_games[i] = new WvsGame(this,channel);
            }
        }

        public void InsertAccount(int accId,int admin,string user,string pass)
        {
            var acc = new Account
            {
                AccId = accId,
                Admin = admin,
                Ban = 0,
                Creation = DateTime.Now,
                LastLogin = DateTime.Now,
                LastIP = string.Empty,
                Username = user,
                Password = pass
            };
            
            Db.Get().GetCollection<Account>("account").InsertOne(acc);
        }

        public void Start()
        {
            m_login.Start();
            m_games.ToList().ForEach(x => x.Start());
        }
        public void Stop()
        {
            m_login.Stop();
            m_games.ToList().ForEach(x => x.Stop());
        }

        public void Dispose()
        {
            WzMan?.Dispose();
            Db?.Dispose();
        }
    }
}
