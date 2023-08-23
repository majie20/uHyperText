using UnityEngine;
using System;
using UnityEngine.UI;

namespace WXB
{
    public interface Owner
    {
        // ��С�и�
        int minLineHeight { get; set; }

        Around around { get; }

        Text self { get; }

        RenderCache renderCache { get; }
        float GetWordSpacing(Font font, bool isText);
        Anchor anchor { get; }

        void SetRenderDirty();

        // Ԫ�طָ�
        ElementSegment elementSegment { get; }

        // ͨ�������ȡ��Ⱦ����,�ῼ�Ǻϲ������
        Draw GetDraw(DrawType type, long key, Action<Draw, object> onCreate, object para = null);

        Material material { get; }

        LineAlignment lineAlignment { get; }

        HorizontalWrapMode HorizontalOverflow { get; }

        VerticalWrapMode VerticalOverflow { get; }
    }
}