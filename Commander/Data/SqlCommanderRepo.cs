using Commander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commander.Data
{
    public class SqlCommanderRepo : ICommandRepo
    {
        private readonly CommanderContext _db;

        public SqlCommanderRepo(CommanderContext db)
        {
            _db = db;
        }

        public void CreateCommand(Command cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }
            _db.Commands.Add(cmd);
        }

        public void DeleteCommand(Command cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }
            _db.Commands.Remove(cmd);
        }

        public IEnumerable<Command> GetAllCommands()
        {
            return _db.Commands.ToList();
        }

        public Command GetCommandById(int id)
        {
            return _db.Commands.FirstOrDefault(c=>c.Id==id);
        }

        public bool SaveChanges()
        {
            return (_db.SaveChanges()>=0);
        }

        public void UpdateCommandById(Command cmd)
        {
            //Nothing
        }
    }
}
