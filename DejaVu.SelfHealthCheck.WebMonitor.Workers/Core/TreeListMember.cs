using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DejaVu.SelfHealthCheck.Contracts;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
    public class TreeListMember : Component
    {
        public virtual CheckResultStatus Status { get; set; }
        public virtual DateTime DateChecked { get; set; }
        public virtual List<ICheckResult> Results { get; set; }
        public virtual string RowClientId { get; set; }
        public virtual string StatusCellClientId { get; set; }
        public virtual string DateCellClientId { get; set; }
        //protected TreeListMember parentCore { get; set; }
        //protected ArrayList childrenCore { get; set; }
        //protected object[] cellsCore;

        //public override string ToString()
        //{
        //    return AppID;
        //}

        //public void SetParentCore(TreeListMember m)
        //{
        //    parentCore = m;
        //}

        //public void SetChildrenCore(List<TreeListMember> c)
        //{
        //    childrenCore = new ArrayList();
        //    foreach (var child in c)
        //    {
        //        childrenCore.Add(child);
        //    }
        //}

        //public void SetCellsCore(object[] cells)
        //{
        //    cellsCore = cells;
        //}

        //public void VirtualTreeGetCellValue(DevExpress.XtraTreeList.VirtualTreeGetCellValueInfo info)
        //{
        //    info.CellData = cellsCore[info.Column.AbsoluteIndex];
        //}

        //public void VirtualTreeGetChildNodes(DevExpress.XtraTreeList.VirtualTreeGetChildNodesInfo info)
        //{
        //    info.Children = childrenCore;
        //}

        //public void VirtualTreeSetCellValue(DevExpress.XtraTreeList.VirtualTreeSetCellValueInfo info)
        //{
        //    cellsCore[info.Column.AbsoluteIndex] = info.NewCellData;
        //}


    }
}
