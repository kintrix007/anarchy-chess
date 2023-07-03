using System;
using System.Collections.Generic;

namespace AnarchyChess.PieceHelper
{
    public interface IPromotable
    {
        List<Type> Promotions { get; }
    }
}
