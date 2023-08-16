﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WXB
{
    class SpriteData
    {
        public Vector2 leftPos;
        public Color color;
        public float width;
        public float height;

        public void Gen(VertexHelper vh, Vector4 uv)
        {
            int count = vh.currentVertCount;
            vh.AddVert(new Vector3(leftPos.x, leftPos.y), color, new Vector2(uv.x, uv.y));
            vh.AddVert(new Vector3(leftPos.x, leftPos.y + height), color, new Vector2(uv.x, uv.w));
            vh.AddVert(new Vector3(leftPos.x + width, leftPos.y + height), color, new Vector2(uv.z, uv.w));
            vh.AddVert(new Vector3(leftPos.x + width, leftPos.y), color, new Vector2(uv.z, uv.y));

            vh.AddTriangle(count, count + 1, count + 2);
            vh.AddTriangle(count + 2, count + 3, count);
        }
    }

    [ExecuteInEditMode]
    public class CartoonDraw : EffectDrawObjec, ICanvasElement
    {
        public override DrawType type { get { return DrawType.Cartoon; } }

        public Cartoon cartoon { get; set; }
        int frameIndex = -1;
        float mDelta = 0f;

        void UpdateAnim(float deltaTime)
        {
            if (frameIndex < 0)
                frameIndex = 0;

            mDelta += Mathf.Min(1f, deltaTime);
            if (cartoon is OCartoon oCartoon)
            {
                if (mDelta >= oCartoon.frame.delay)
                {
                    mDelta -= oCartoon.frame.delay;

                    if (++frameIndex >= oCartoon.row * oCartoon.col)
                    {
                        frameIndex = 0;
                    }
                }
            }
            else
            {
                var frames = cartoon.frames;
                var frame = frames[frameIndex];
                while (mDelta >= frame.delay)
                {
                    mDelta -= frame.delay;
                    if (++frameIndex >= frames.Length)
                    {
                        frameIndex = 0;
                    }

                    frame = frames[frameIndex];
                }
            }
        }

        List<SpriteData> mData = new List<SpriteData>();

        public bool isOpenAlpha
        {
            get { return GetOpen(0); }
            set { SetOpen<AlphaEffect>(0, value); }
        }

        public bool isOpenOffset
        {
            get { return GetOpen(1); }
            set { SetOpen<OffsetEffect>(1, value); }
        }

        public void Add(Vector2 leftPos, float width, float height, Color color)
        {
            var sd = PoolData<SpriteData>.Get();
            sd.leftPos = leftPos;
            sd.color = color;
            sd.width = width;
            sd.height = height;

            mData.Add(sd);
        }

        public override void UpdateSelf(float deltaTime)
        {
            base.UpdateSelf(deltaTime);
            int f = frameIndex;
            UpdateAnim(deltaTime);

            if (cartoon is OCartoon oCartoon)
            {
                if (f != frameIndex || (currentIsEmpty && oCartoon.frame.sprite.Get() != null))
                {
                    CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
                    return;
                }
            }
            else
            {
                if (f != frameIndex || (currentIsEmpty && cartoon.frames[f].sprite.Get() != null))
                {
                    CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
                    return;
                }
            }
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
            }
#endif
        }

        bool currentIsEmpty = false;
        public void Rebuild(CanvasUpdate executing)
        {
            if (executing != CanvasUpdate.PreRender)
                return;

            if (mData == null)
                return;

            if (frameIndex < 0)
                frameIndex = 0;

            ISprite si;
            OCartoon oCartoon;
            if (cartoon is OCartoon)
            {
                oCartoon = (OCartoon) cartoon;
                si = oCartoon.frame.sprite;
            }
            else
            {
                oCartoon = null;
                si = cartoon.frames[frameIndex].sprite; 
            }
            Sprite s = si.Get();
            if (s == null)
            {
                currentIsEmpty = true;
                return;
            }

            currentIsEmpty = false;
            var uv = UnityEngine.Sprites.DataUtility.GetOuterUV(s);
            if (oCartoon != null)
            {
                uv.z = 1.0f / oCartoon.col;
                uv.w = 1.0f / oCartoon.row;
                uv.x = frameIndex % oCartoon.col * uv.z;
                uv.y = frameIndex / oCartoon.col * uv.w;
                uv.z += uv.x;
                uv.w += uv.y;
            }
            VertexHelper vh = Tools.vertexHelper;
            vh.Clear();
            for (int i = 0; i < mData.Count; ++i)
            {
                mData[i].Gen(vh, uv);
            }

            Mesh workerMesh = SymbolText.WorkerMesh;
            vh.FillMesh(workerMesh);
            canvasRenderer.SetMesh(workerMesh);
            canvasRenderer.SetTexture(s.texture);
        }

        public override void Release()
        {
            base.Release();
            PoolData<SpriteData>.FreeList(mData);
            frameIndex = -1;
        }

        public void GraphicUpdateComplete() { }
        public bool IsDestroyed() { return this == null; }
        public void LayoutComplete() { }

        public override void OnCullingChanged()
        {
            if (!canvasRenderer.cull)
            {
                /// When we were culled, we potentially skipped calls to <c>Rebuild</c>.
                CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
            }
        }
    }
}