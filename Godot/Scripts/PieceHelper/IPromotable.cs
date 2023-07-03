using System;
using System.Collections.Generic;

namespace AnarchyChess.Scripts.PieceHelper
{
    public interface IPromotable
    {
        List<Type> Promotions { get; }
    }
}
