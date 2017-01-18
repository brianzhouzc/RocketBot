using System.ComponentModel;
using System.Reflection;

namespace PoGo.NecroBot.Logic.Model.Settings
{
    public class BaseConfig
    {
        public BaseConfig()
        {
            PropertyInfo[] props = this.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                var d = prop.GetCustomAttribute<DefaultValueAttribute>();
                if (d != null)
                    prop.SetValue(this, d.Value);
            }
        }
    }
}