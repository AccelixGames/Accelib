using Accelib.Module.AccelNovel.Control.Action.Sprite.Base;
using Accelix.Accelib.AccelNovel.Model.Enum;

namespace Accelib.Module.AccelNovel.Control.Action.Sprite
{
    public class ActionController_CG : ActionController_Sprite
    {
        public override string Keyword => "cg";
        protected override ENextActionMode DefaultNextActionMode => ENextActionMode.Auto;
        protected override bool IsUnlockCollective => true;
    }
}