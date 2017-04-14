using global::NLog.Layouts;

namespace Tact.NLog.Layouts
{
    [Layout(nameof(TactLayout))]
    public class TactLayout : SimpleLayout
    {
        private const string Txt = "${longdate}|${level:uppercase=true}|${logger}|${message}${onexception:inner=|${cleanEx:separator= - :format=Type,Message,StackTrace}}";

        public TactLayout()
            : base(Txt)
        {
        }
    }
}
