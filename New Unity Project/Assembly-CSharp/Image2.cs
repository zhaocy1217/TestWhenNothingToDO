using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

[AddComponentMenu("UI/Image2", 11)]
public class Image2 : Image
{
    [SerializeField]
    protected ImageAlphaTexLayout m_alphaTexLayout;
    private bool m_initialized;
    private static List<Component> s_components = new List<Component>();
    private static Material s_defaultMaterial;
    private static DictionaryObjectView<Material, Material> s_materialList = new DictionaryObjectView<Material, Material>();
    private static Vector2[] s_sizeScaling = new Vector2[] { new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(1f, 0.5f) };
    private static readonly Vector2[] s_Uv = new Vector2[4];
    private static readonly Vector2[] s_UVScratch = new Vector2[4];
    private static readonly Vector2[] s_VertScratch = new Vector2[4];
    private static readonly Vector2[] s_Xy = new Vector2[4];
    public bool WriteTexcoordToNormal;

    private void AddQuad(List<UIVertex> vbo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax)
    {
        v.position = new Vector3(posMin.x, posMin.y, 0f);
        v.uv0 = new Vector2(uvMin.x, uvMin.y);
        vbo.Add(v);
        v.position = new Vector3(posMin.x, posMax.y, 0f);
        v.uv0 = new Vector2(uvMin.x, uvMax.y);
        vbo.Add(v);
        v.position = new Vector3(posMax.x, posMax.y, 0f);
        v.uv0 = new Vector2(uvMax.x, uvMax.y);
        vbo.Add(v);
        v.position = new Vector3(posMax.x, posMin.y, 0f);
        v.uv0 = new Vector2(uvMax.x, uvMin.y);
        vbo.Add(v);
    }

    private void AddQuad(List<UIVertex> vbo, UIVertex v, Vector2 posMin, Vector2 posMax, Vector2 uvMin, Vector2 uvMax, Vector2 offset)
    {
        v.position = new Vector3(posMin.x, posMin.y, 0f);
        v.uv0 = new Vector2(uvMin.x, uvMin.y);
        v.uv1 = v.uv0 + offset;
        vbo.Add(v);
        v.position = new Vector3(posMin.x, posMax.y, 0f);
        v.uv0 = new Vector2(uvMin.x, uvMax.y);
        v.uv1 = v.uv0 + offset;
        vbo.Add(v);
        v.position = new Vector3(posMax.x, posMax.y, 0f);
        v.uv0 = new Vector2(uvMax.x, uvMax.y);
        v.uv1 = v.uv0 + offset;
        vbo.Add(v);
        v.position = new Vector3(posMax.x, posMin.y, 0f);
        v.uv0 = new Vector2(uvMax.x, uvMin.y);
        v.uv1 = v.uv0 + offset;
        vbo.Add(v);
    }

    private void GenerateFilledSprite(List<UIVertex> vbo, bool preserveAspect)
    {
        if (base.get_fillAmount() >= 0.001)
        {
            Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect, s_sizeScaling[(int) this.alphaTexLayout]);
            Vector2 offset = Vector2.get_zero();
            Vector4 vector3 = Vector4.get_zero();
            if (base.get_overrideSprite() != null)
            {
                vector3 = GetOuterUV(base.get_overrideSprite(), this.alphaTexLayout, out offset);
            }
            UIVertex simpleVert = UIVertex.simpleVert;
            simpleVert.color = base.get_color();
            float x = vector3.x;
            float y = vector3.y;
            float z = vector3.z;
            float w = vector3.w;
            if ((base.get_fillMethod() == null) || (base.get_fillMethod() == 1))
            {
                if (base.get_fillMethod() == null)
                {
                    float num5 = (z - x) * base.get_fillAmount();
                    if (base.get_fillOrigin() == 1)
                    {
                        drawingDimensions.x = drawingDimensions.z - ((drawingDimensions.z - drawingDimensions.x) * base.get_fillAmount());
                        x = z - num5;
                    }
                    else
                    {
                        drawingDimensions.z = drawingDimensions.x + ((drawingDimensions.z - drawingDimensions.x) * base.get_fillAmount());
                        z = x + num5;
                    }
                }
                else if (base.get_fillMethod() == 1)
                {
                    float num6 = (w - y) * base.get_fillAmount();
                    if (base.get_fillOrigin() == 1)
                    {
                        drawingDimensions.y = drawingDimensions.w - ((drawingDimensions.w - drawingDimensions.y) * base.get_fillAmount());
                        y = w - num6;
                    }
                    else
                    {
                        drawingDimensions.w = drawingDimensions.y + ((drawingDimensions.w - drawingDimensions.y) * base.get_fillAmount());
                        w = y + num6;
                    }
                }
            }
            s_Xy[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
            s_Xy[1] = new Vector2(drawingDimensions.x, drawingDimensions.w);
            s_Xy[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
            s_Xy[3] = new Vector2(drawingDimensions.z, drawingDimensions.y);
            s_Uv[0] = new Vector2(x, y);
            s_Uv[1] = new Vector2(x, w);
            s_Uv[2] = new Vector2(z, w);
            s_Uv[3] = new Vector2(z, y);
            if (base.get_fillAmount() < 1.0)
            {
                if (base.get_fillMethod() == 2)
                {
                    if (RadialCut(s_Xy, s_Uv, base.get_fillAmount(), base.get_fillClockwise(), base.get_fillOrigin()))
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            simpleVert.position = s_Xy[j];
                            simpleVert.uv0 = s_Uv[j];
                            simpleVert.uv1 = simpleVert.uv0 + offset;
                            vbo.Add(simpleVert);
                        }
                    }
                    return;
                }
                if (base.get_fillMethod() == 3)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        float num10;
                        float num11;
                        float num12;
                        float num13;
                        int num9 = (base.get_fillOrigin() > 1) ? 1 : 0;
                        if ((base.get_fillOrigin() == 0) || (base.get_fillOrigin() == 2))
                        {
                            num10 = 0f;
                            num11 = 1f;
                            if (k == num9)
                            {
                                num12 = 0f;
                                num13 = 0.5f;
                            }
                            else
                            {
                                num12 = 0.5f;
                                num13 = 1f;
                            }
                        }
                        else
                        {
                            num12 = 0f;
                            num13 = 1f;
                            if (k == num9)
                            {
                                num10 = 0.5f;
                                num11 = 1f;
                            }
                            else
                            {
                                num10 = 0f;
                                num11 = 0.5f;
                            }
                        }
                        s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num12);
                        s_Xy[1].x = s_Xy[0].x;
                        s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num13);
                        s_Xy[3].x = s_Xy[2].x;
                        s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num10);
                        s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num11);
                        s_Xy[2].y = s_Xy[1].y;
                        s_Xy[3].y = s_Xy[0].y;
                        s_Uv[0].x = Mathf.Lerp(x, z, num12);
                        s_Uv[1].x = s_Uv[0].x;
                        s_Uv[2].x = Mathf.Lerp(x, z, num13);
                        s_Uv[3].x = s_Uv[2].x;
                        s_Uv[0].y = Mathf.Lerp(y, w, num10);
                        s_Uv[1].y = Mathf.Lerp(y, w, num11);
                        s_Uv[2].y = s_Uv[1].y;
                        s_Uv[3].y = s_Uv[0].y;
                        float num14 = base.get_fillClockwise() ? ((base.get_fillAmount() * 2f) - k) : ((base.get_fillAmount() * 2f) - (1 - k));
                        if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(num14), base.get_fillClockwise(), ((k + base.get_fillOrigin()) + 3) % 4))
                        {
                            for (int m = 0; m < 4; m++)
                            {
                                simpleVert.position = s_Xy[m];
                                simpleVert.uv0 = s_Uv[m];
                                simpleVert.uv1 = simpleVert.uv0 + offset;
                                vbo.Add(simpleVert);
                            }
                        }
                    }
                    return;
                }
                if (base.get_fillMethod() == 4)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        float num17;
                        float num18;
                        float num19;
                        float num20;
                        if (n < 2)
                        {
                            num17 = 0f;
                            num18 = 0.5f;
                        }
                        else
                        {
                            num17 = 0.5f;
                            num18 = 1f;
                        }
                        switch (n)
                        {
                            case 0:
                            case 3:
                                num19 = 0f;
                                num20 = 0.5f;
                                break;

                            default:
                                num19 = 0.5f;
                                num20 = 1f;
                                break;
                        }
                        s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num17);
                        s_Xy[1].x = s_Xy[0].x;
                        s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num18);
                        s_Xy[3].x = s_Xy[2].x;
                        s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num19);
                        s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num20);
                        s_Xy[2].y = s_Xy[1].y;
                        s_Xy[3].y = s_Xy[0].y;
                        s_Uv[0].x = Mathf.Lerp(x, z, num17);
                        s_Uv[1].x = s_Uv[0].x;
                        s_Uv[2].x = Mathf.Lerp(x, z, num18);
                        s_Uv[3].x = s_Uv[2].x;
                        s_Uv[0].y = Mathf.Lerp(y, w, num19);
                        s_Uv[1].y = Mathf.Lerp(y, w, num20);
                        s_Uv[2].y = s_Uv[1].y;
                        s_Uv[3].y = s_Uv[0].y;
                        float num21 = base.get_fillClockwise() ? ((base.get_fillAmount() * 4f) - ((n + base.get_fillOrigin()) % 4)) : ((base.get_fillAmount() * 4f) - (3 - ((n + base.get_fillOrigin()) % 4)));
                        if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(num21), base.get_fillClockwise(), (n + 2) % 4))
                        {
                            for (int num22 = 0; num22 < 4; num22++)
                            {
                                simpleVert.position = s_Xy[num22];
                                simpleVert.uv0 = s_Uv[num22];
                                simpleVert.uv1 = simpleVert.uv0 + offset;
                                vbo.Add(simpleVert);
                            }
                        }
                    }
                    return;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                simpleVert.position = s_Xy[i];
                simpleVert.uv0 = s_Uv[i];
                simpleVert.uv1 = simpleVert.uv0 + offset;
                vbo.Add(simpleVert);
            }
        }
    }

    private void GenerateSimpleSprite(List<UIVertex> vbo, bool preserveAspect)
    {
        Vector2 sizeScaling = s_sizeScaling[(int) this.alphaTexLayout];
        UIVertex simpleVert = UIVertex.simpleVert;
        simpleVert.color = base.get_color();
        Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect, sizeScaling);
        Vector4 vector3 = (base.get_overrideSprite() != null) ? DataUtility.GetOuterUV(base.get_overrideSprite()) : Vector4.get_zero();
        float y = vector3.y;
        float w = vector3.w;
        float num3 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? w : ((y + w) * 0.5f);
        float num4 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? y : ((y + w) * 0.5f);
        float x = vector3.x;
        float z = vector3.z;
        float num7 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? z : ((x + z) * 0.5f);
        float num8 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? x : ((x + z) * 0.5f);
        simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.y);
        simpleVert.uv0 = new Vector2(x, y);
        simpleVert.uv1 = new Vector2(num8, num4);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.w);
        simpleVert.uv0 = new Vector2(x, num3);
        simpleVert.uv1 = new Vector2(num8, w);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.w);
        simpleVert.uv0 = new Vector2(num7, num3);
        simpleVert.uv1 = new Vector2(z, w);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.y);
        simpleVert.uv0 = new Vector2(num7, y);
        simpleVert.uv1 = new Vector2(z, num4);
        vbo.Add(simpleVert);
    }

    private void GenerateSimpleSprite_Normal(List<UIVertex> vbo, bool preserveAspect)
    {
        Vector2 sizeScaling = s_sizeScaling[(int) this.alphaTexLayout];
        UIVertex simpleVert = UIVertex.simpleVert;
        simpleVert.color = base.get_color();
        Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect, sizeScaling);
        Vector4 vector3 = (base.get_overrideSprite() != null) ? DataUtility.GetOuterUV(base.get_overrideSprite()) : Vector4.get_zero();
        float y = vector3.y;
        float w = vector3.w;
        float num3 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? w : ((y + w) * 0.5f);
        float num4 = (this.alphaTexLayout != ImageAlphaTexLayout.Vertical) ? y : ((y + w) * 0.5f);
        float x = vector3.x;
        float z = vector3.z;
        float num7 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? z : ((x + z) * 0.5f);
        float num8 = (this.alphaTexLayout != ImageAlphaTexLayout.Horizonatal) ? x : ((x + z) * 0.5f);
        simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.y);
        simpleVert.uv0 = new Vector2(x, y);
        simpleVert.uv1 = new Vector2(num8, num4);
        simpleVert.normal = new Vector3(-1f, -1f, 0f);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.x, drawingDimensions.w);
        simpleVert.uv0 = new Vector2(x, num3);
        simpleVert.uv1 = new Vector2(num8, w);
        simpleVert.normal = new Vector3(-1f, 1f, 0f);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.w);
        simpleVert.uv0 = new Vector2(num7, num3);
        simpleVert.uv1 = new Vector2(z, w);
        simpleVert.normal = new Vector3(1f, 1f, 0f);
        vbo.Add(simpleVert);
        simpleVert.position = new Vector3(drawingDimensions.z, drawingDimensions.y);
        simpleVert.uv0 = new Vector2(num7, y);
        simpleVert.uv1 = new Vector2(z, num4);
        simpleVert.normal = new Vector3(1f, -1f, 0f);
        vbo.Add(simpleVert);
    }

    private void GenerateSlicedSprite(List<UIVertex> vbo)
    {
        if (!base.get_hasBorder())
        {
            this.GenerateSimpleSprite(vbo, false);
        }
        else
        {
            Vector4 vector;
            Vector4 innerUV;
            Vector4 padding;
            Vector4 vector4;
            Vector2 offset = Vector2.get_zero();
            if (base.get_overrideSprite() != null)
            {
                vector = GetOuterUV(base.get_overrideSprite(), this.alphaTexLayout, out offset);
                innerUV = GetInnerUV(base.get_overrideSprite(), s_sizeScaling[(int) this.alphaTexLayout]);
                padding = DataUtility.GetPadding(base.get_overrideSprite());
                vector4 = base.get_overrideSprite().get_border();
            }
            else
            {
                vector = Vector4.get_zero();
                innerUV = Vector4.get_zero();
                padding = Vector4.get_zero();
                vector4 = Vector4.get_zero();
            }
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            Vector4 adjustedBorders = this.GetAdjustedBorders((Vector4) (vector4 / base.get_pixelsPerUnit()), pixelAdjustedRect);
            Vector4 vector7 = (Vector4) (padding / base.get_pixelsPerUnit());
            s_VertScratch[0] = new Vector2(vector7.x, vector7.y);
            s_VertScratch[3] = new Vector2(pixelAdjustedRect.get_width() - vector7.z, pixelAdjustedRect.get_height() - vector7.w);
            s_VertScratch[1].x = adjustedBorders.x;
            s_VertScratch[1].y = adjustedBorders.y;
            s_VertScratch[2].x = pixelAdjustedRect.get_width() - adjustedBorders.z;
            s_VertScratch[2].y = pixelAdjustedRect.get_height() - adjustedBorders.w;
            for (int i = 0; i < 4; i++)
            {
                s_VertScratch[i].x += pixelAdjustedRect.get_x();
                s_VertScratch[i].y += pixelAdjustedRect.get_y();
            }
            s_UVScratch[0] = new Vector2(vector.x, vector.y);
            s_UVScratch[1] = new Vector2(innerUV.x, innerUV.y);
            s_UVScratch[2] = new Vector2(innerUV.z, innerUV.w);
            s_UVScratch[3] = new Vector2(vector.z, vector.w);
            UIVertex simpleVert = UIVertex.simpleVert;
            simpleVert.color = base.get_color();
            for (int j = 0; j < 3; j++)
            {
                int index = j + 1;
                for (int k = 0; k < 3; k++)
                {
                    if ((base.get_fillCenter() || (j != 1)) || (k != 1))
                    {
                        int num5 = k + 1;
                        this.AddQuad(vbo, simpleVert, new Vector2(s_VertScratch[j].x, s_VertScratch[k].y), new Vector2(s_VertScratch[index].x, s_VertScratch[num5].y), new Vector2(s_UVScratch[j].x, s_UVScratch[k].y), new Vector2(s_UVScratch[index].x, s_UVScratch[num5].y), offset);
                    }
                }
            }
        }
    }

    private void GenerateTiledSprite(List<UIVertex> vbo)
    {
        Vector4 vector;
        Vector4 innerUV;
        Vector4 adjustedBorders;
        Vector2 vector4;
        Vector2 vector5;
        if (base.get_overrideSprite() != null)
        {
            Vector2 sizeScaling = s_sizeScaling[(int) this.alphaTexLayout];
            vector = GetOuterUV(base.get_overrideSprite(), this.alphaTexLayout, out vector5);
            innerUV = GetInnerUV(base.get_overrideSprite(), sizeScaling);
            adjustedBorders = base.get_overrideSprite().get_border();
            vector4 = base.get_overrideSprite().get_rect().get_size();
            vector4.x *= sizeScaling.x;
            vector4.y *= sizeScaling.y;
        }
        else
        {
            vector = Vector4.get_zero();
            innerUV = Vector4.get_zero();
            adjustedBorders = Vector4.get_zero();
            vector4 = (Vector2) (Vector2.get_one() * 100f);
            vector5 = Vector2.get_zero();
        }
        Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
        float num = ((vector4.x - adjustedBorders.x) - adjustedBorders.z) / base.get_pixelsPerUnit();
        float num2 = ((vector4.y - adjustedBorders.y) - adjustedBorders.w) / base.get_pixelsPerUnit();
        adjustedBorders = this.GetAdjustedBorders((Vector4) (adjustedBorders / base.get_pixelsPerUnit()), pixelAdjustedRect);
        Vector2 uvMin = new Vector2(innerUV.x, innerUV.y);
        Vector2 vector8 = new Vector2(innerUV.z, innerUV.w);
        UIVertex simpleVert = UIVertex.simpleVert;
        simpleVert.color = base.get_color();
        float x = adjustedBorders.x;
        float num4 = pixelAdjustedRect.get_width() - adjustedBorders.z;
        float y = adjustedBorders.y;
        float num6 = pixelAdjustedRect.get_height() - adjustedBorders.w;
        if (((num4 - x) > (num * 100.0)) || ((num6 - y) > (num2 * 100.0)))
        {
            num = (float) ((num4 - x) / 100.0);
            num2 = (float) ((num6 - y) / 100.0);
        }
        Vector2 uvMax = vector8;
        if (base.get_fillCenter())
        {
            for (float i = y; i < num6; i += num2)
            {
                float num8 = i + num2;
                if (num8 > num6)
                {
                    uvMax.y = uvMin.y + (((vector8.y - uvMin.y) * (num6 - i)) / (num8 - i));
                    num8 = num6;
                }
                uvMax.x = vector8.x;
                for (float j = x; j < num4; j += num)
                {
                    float num10 = j + num;
                    if (num10 > num4)
                    {
                        uvMax.x = uvMin.x + (((vector8.x - uvMin.x) * (num4 - j)) / (num10 - j));
                        num10 = num4;
                    }
                    this.AddQuad(vbo, simpleVert, new Vector2(j, i) + pixelAdjustedRect.get_position(), new Vector2(num10, num8) + pixelAdjustedRect.get_position(), uvMin, uvMax, vector5);
                }
            }
        }
        if (base.get_hasBorder())
        {
            Vector2 vector10 = vector8;
            for (float k = y; k < num6; k += num2)
            {
                float num12 = k + num2;
                if (num12 > num6)
                {
                    vector10.y = uvMin.y + (((vector8.y - uvMin.y) * (num6 - k)) / (num12 - k));
                    num12 = num6;
                }
                this.AddQuad(vbo, simpleVert, new Vector2(0f, k) + pixelAdjustedRect.get_position(), new Vector2(x, num12) + pixelAdjustedRect.get_position(), new Vector2(vector.x, uvMin.y), new Vector2(uvMin.x, vector10.y), vector5);
                this.AddQuad(vbo, simpleVert, new Vector2(num4, k) + pixelAdjustedRect.get_position(), new Vector2(pixelAdjustedRect.get_width(), num12) + pixelAdjustedRect.get_position(), new Vector2(vector8.x, uvMin.y), new Vector2(vector.z, vector10.y), vector5);
            }
            vector10 = vector8;
            for (float m = x; m < num4; m += num)
            {
                float num14 = m + num;
                if (num14 > num4)
                {
                    vector10.x = uvMin.x + (((vector8.x - uvMin.x) * (num4 - m)) / (num14 - m));
                    num14 = num4;
                }
                this.AddQuad(vbo, simpleVert, new Vector2(m, 0f) + pixelAdjustedRect.get_position(), new Vector2(num14, y) + pixelAdjustedRect.get_position(), new Vector2(uvMin.x, vector.y), new Vector2(vector10.x, uvMin.y), vector5);
                this.AddQuad(vbo, simpleVert, new Vector2(m, num6) + pixelAdjustedRect.get_position(), new Vector2(num14, pixelAdjustedRect.get_height()) + pixelAdjustedRect.get_position(), new Vector2(uvMin.x, vector8.y), new Vector2(vector10.x, vector.w), vector5);
            }
            this.AddQuad(vbo, simpleVert, new Vector2(0f, 0f) + pixelAdjustedRect.get_position(), new Vector2(x, y) + pixelAdjustedRect.get_position(), new Vector2(vector.x, vector.y), new Vector2(uvMin.x, uvMin.y), vector5);
            this.AddQuad(vbo, simpleVert, new Vector2(num4, 0f) + pixelAdjustedRect.get_position(), new Vector2(pixelAdjustedRect.get_width(), y) + pixelAdjustedRect.get_position(), new Vector2(vector8.x, vector.y), new Vector2(vector.z, uvMin.y), vector5);
            this.AddQuad(vbo, simpleVert, new Vector2(0f, num6) + pixelAdjustedRect.get_position(), new Vector2(x, pixelAdjustedRect.get_height()) + pixelAdjustedRect.get_position(), new Vector2(vector.x, vector8.y), new Vector2(uvMin.x, vector.w), vector5);
            this.AddQuad(vbo, simpleVert, new Vector2(num4, num6) + pixelAdjustedRect.get_position(), new Vector2(pixelAdjustedRect.get_width(), pixelAdjustedRect.get_height()) + pixelAdjustedRect.get_position(), new Vector2(vector8.x, vector8.y), new Vector2(vector.z, vector.w), vector5);
        }
    }

    private unsafe Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
    {
        for (int i = 0; i <= 1; i++)
        {
            float num2 = border.get_Item(i) + border.get_Item(i + 2);
            if ((rect.get_size().get_Item(i) < num2) && (num2 != 0.0))
            {
                ref Vector4 vectorRef;
                int num4;
                ref Vector4 vectorRef2;
                float num3 = rect.get_size().get_Item(i) / num2;
                (vectorRef = &border).set_Item(num4 = i, vectorRef.get_Item(num4) * num3);
                (vectorRef2 = &border).set_Item(num4 = i + 2, vectorRef2.get_Item(num4) * num3);
            }
        }
        return border;
    }

    private Vector4 GetDrawingDimensions(bool shouldPreserveAspect, Vector2 sizeScaling)
    {
        Vector4 vector = (base.get_overrideSprite() == null) ? Vector4.get_zero() : DataUtility.GetPadding(base.get_overrideSprite());
        Vector2 vector2 = (base.get_overrideSprite() == null) ? Vector2.get_zero() : new Vector2(base.get_overrideSprite().get_rect().get_width() * sizeScaling.x, base.get_overrideSprite().get_rect().get_height() * sizeScaling.y);
        Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
        int num = Mathf.RoundToInt(vector2.x);
        int num2 = Mathf.RoundToInt(vector2.y);
        Vector4 vector3 = new Vector4(vector.x / ((float) num), vector.y / ((float) num2), (num - vector.z) / ((float) num), (num2 - vector.w) / ((float) num2));
        if (shouldPreserveAspect && (vector2.get_sqrMagnitude() > 0.0))
        {
            float num3 = vector2.x / vector2.y;
            float num4 = pixelAdjustedRect.get_width() / pixelAdjustedRect.get_height();
            if (num3 > num4)
            {
                float num5 = pixelAdjustedRect.get_height();
                pixelAdjustedRect.set_height(pixelAdjustedRect.get_width() * (1f / num3));
                pixelAdjustedRect.set_y(pixelAdjustedRect.get_y() + ((num5 - pixelAdjustedRect.get_height()) * base.get_rectTransform().get_pivot().y));
            }
            else
            {
                float num6 = pixelAdjustedRect.get_width();
                pixelAdjustedRect.set_width(pixelAdjustedRect.get_height() * num3);
                pixelAdjustedRect.set_x(pixelAdjustedRect.get_x() + ((num6 - pixelAdjustedRect.get_width()) * base.get_rectTransform().get_pivot().x));
            }
        }
        return new Vector4(pixelAdjustedRect.get_x() + (pixelAdjustedRect.get_width() * vector3.x), pixelAdjustedRect.get_y() + (pixelAdjustedRect.get_height() * vector3.y), pixelAdjustedRect.get_x() + (pixelAdjustedRect.get_width() * vector3.z), pixelAdjustedRect.get_y() + (pixelAdjustedRect.get_height() * vector3.w));
    }

    private static Vector4 GetInnerUV(Sprite sprite, Vector2 sizeScaling)
    {
        Texture texture = sprite.get_texture();
        if (texture == null)
        {
            return new Vector4(0f, 0f, sizeScaling.x, sizeScaling.y);
        }
        Rect rect = sprite.get_textureRect();
        rect.set_width(rect.get_width() * sizeScaling.x);
        rect.set_height(rect.get_height() * sizeScaling.y);
        float num = 1f / ((float) texture.get_width());
        float num2 = 1f / ((float) texture.get_height());
        Vector4 padding = DataUtility.GetPadding(sprite);
        Vector4 vector2 = sprite.get_border();
        float num3 = rect.get_x() + padding.x;
        float num4 = rect.get_y() + padding.y;
        Vector4 vector3 = new Vector4();
        vector3.x = num3 + vector2.x;
        vector3.y = num4 + vector2.y;
        vector3.z = (rect.get_x() + rect.get_width()) - vector2.z;
        vector3.w = (rect.get_y() + rect.get_height()) - vector2.w;
        vector3.x *= num;
        vector3.y *= num2;
        vector3.z *= num;
        vector3.w *= num2;
        return vector3;
    }

    private static Vector4 GetOuterUV(Sprite sprite, ImageAlphaTexLayout layout, out Vector2 offset)
    {
        Vector4 outerUV = DataUtility.GetOuterUV(sprite);
        offset = Vector2.get_zero();
        ImageAlphaTexLayout layout2 = layout;
        if (layout2 != ImageAlphaTexLayout.Horizonatal)
        {
            if (layout2 != ImageAlphaTexLayout.Vertical)
            {
                return outerUV;
            }
        }
        else
        {
            offset.x = (outerUV.z - outerUV.x) * 0.5f;
            outerUV.z = (outerUV.z + outerUV.x) * 0.5f;
            return outerUV;
        }
        offset.y = (outerUV.w - outerUV.y) * 0.5f;
        outerUV.w = (outerUV.w + outerUV.y) * 0.5f;
        return outerUV;
    }

    private int GetStencilForGraphic()
    {
        int num = 0;
        Transform transform = base.get_transform().get_parent();
        s_components.Clear();
        while (transform != null)
        {
            transform.GetComponents(typeof(IMask), s_components);
            for (int i = 0; i < s_components.Count; i++)
            {
                IMask mask = s_components[i] as IMask;
                if ((mask != null) && mask.MaskEnabled())
                {
                    num++;
                    num = Mathf.Clamp(num, 0, 8);
                    break;
                }
            }
            transform = transform.get_parent();
        }
        s_components.Clear();
        return num;
    }

    protected override void OnCanvasHierarchyChanged()
    {
    }

    protected override void OnDestroy()
    {
        base.set_sprite(null);
        base.set_overrideSprite(null);
    }

    protected override void OnFillVBO(List<UIVertex> vbo)
    {
        if ((base.get_overrideSprite() == null) || ((this.alphaTexLayout == ImageAlphaTexLayout.None) && !this.WriteTexcoordToNormal))
        {
            base.OnFillVBO(vbo);
        }
        else
        {
            switch (base.get_type())
            {
                case 0:
                    if (!this.WriteTexcoordToNormal)
                    {
                        this.GenerateSimpleSprite(vbo, base.get_preserveAspect());
                        return;
                    }
                    this.GenerateSimpleSprite_Normal(vbo, base.get_preserveAspect());
                    return;

                case 1:
                    this.GenerateSlicedSprite(vbo);
                    return;

                case 2:
                    this.GenerateTiledSprite(vbo);
                    return;

                case 3:
                    this.GenerateFilledSprite(vbo, base.get_preserveAspect());
                    return;

                default:
                    DebugHelper.Assert(false);
                    return;
            }
        }
    }

    private static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
    {
        int index = corner;
        int num2 = (corner + 1) % 4;
        int num3 = (corner + 2) % 4;
        int num4 = (corner + 3) % 4;
        if ((corner & 1) == 1)
        {
            if (sin > cos)
            {
                cos /= sin;
                sin = 1f;
                if (invert)
                {
                    xy[num2].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                    xy[num3].x = xy[num2].x;
                }
            }
            else if (cos > sin)
            {
                sin /= cos;
                cos = 1f;
                if (!invert)
                {
                    xy[num3].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                    xy[num4].y = xy[num3].y;
                }
            }
            else
            {
                cos = 1f;
                sin = 1f;
            }
            if (!invert)
            {
                xy[num4].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
            }
            else
            {
                xy[num2].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
            }
        }
        else
        {
            if (cos > sin)
            {
                sin /= cos;
                cos = 1f;
                if (!invert)
                {
                    xy[num2].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                    xy[num3].y = xy[num2].y;
                }
            }
            else if (sin > cos)
            {
                cos /= sin;
                sin = 1f;
                if (invert)
                {
                    xy[num3].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                    xy[num4].x = xy[num3].x;
                }
            }
            else
            {
                cos = 1f;
                sin = 1f;
            }
            if (invert)
            {
                xy[num4].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
            }
            else
            {
                xy[num2].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
            }
        }
    }

    private static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
    {
        if (fill < 0.001)
        {
            return false;
        }
        if ((corner & 1) == 1)
        {
            invert = !invert;
        }
        if (invert || (fill <= 0.999000012874603))
        {
            float num = Mathf.Clamp01(fill);
            if (invert)
            {
                num = 1f - num;
            }
            float num2 = num * 1.570796f;
            float cos = Mathf.Cos(num2);
            float sin = Mathf.Sin(num2);
            RadialCut(xy, cos, sin, invert, corner);
            RadialCut(uv, cos, sin, invert, corner);
        }
        return true;
    }

    public void SetMaterialVector(string name, Vector4 factor)
    {
        if (base.m_Material != null)
        {
            if (!base.m_Material.get_name().Contains("(Clone)"))
            {
                Material material = new Material(base.m_Material);
                material.set_name(base.m_Material.get_name() + "(Clone)");
                material.CopyPropertiesFromMaterial(base.m_Material);
                material.set_shaderKeywords(base.m_Material.get_shaderKeywords());
                material.SetVector(name, factor);
                this.material = material;
            }
            else
            {
                base.m_Material.SetVector(name, factor);
                this.SetMaterialDirty();
            }
        }
    }

    public override void SetNativeSize()
    {
        if (base.get_overrideSprite() != null)
        {
            Vector2 vector = s_sizeScaling[(int) this.alphaTexLayout];
            float num = (base.get_overrideSprite().get_rect().get_width() * vector.x) / base.get_pixelsPerUnit();
            float num2 = (base.get_overrideSprite().get_rect().get_height() * vector.y) / base.get_pixelsPerUnit();
            base.get_rectTransform().set_anchorMax(base.get_rectTransform().get_anchorMin());
            base.get_rectTransform().set_sizeDelta(new Vector2(num, num2));
            this.SetAllDirty();
        }
    }

    private void UpdateInternalState()
    {
        if (base.m_ShouldRecalculate)
        {
            base.m_StencilValue = this.GetStencilForGraphic();
            Transform transform = base.get_transform().get_parent();
            base.m_IncludeForMasking = false;
            s_components.Clear();
            while (base.get_maskable() && (transform != null))
            {
                transform.GetComponents(typeof(IMask), s_components);
                if (s_components.Count > 0)
                {
                    base.m_IncludeForMasking = true;
                    break;
                }
                transform = transform.get_parent();
            }
            base.m_ShouldRecalculate = false;
            s_components.Clear();
        }
    }

    public ImageAlphaTexLayout alphaTexLayout
    {
        get
        {
            return this.m_alphaTexLayout;
        }
        set
        {
            if (this.m_alphaTexLayout != value)
            {
                this.m_alphaTexLayout = value;
                this.SetMaterialDirty();
            }
        }
    }

    public Material baseMaterial
    {
        get
        {
            if ((base.m_Material == null) || (base.m_Material == defaultMaterial))
            {
                return ((this.alphaTexLayout != ImageAlphaTexLayout.None) ? defaultMaterial : Graphic.get_defaultGraphicMaterial());
            }
            if (this.alphaTexLayout == ImageAlphaTexLayout.None)
            {
                return base.m_Material;
            }
            Material material = null;
            if (!s_materialList.TryGetValue(base.m_Material, out material))
            {
                material = new Material(base.m_Material);
                material.set_shaderKeywords(base.m_Material.get_shaderKeywords());
                material.EnableKeyword("_ALPHATEX_ON");
                s_materialList.Add(base.m_Material, material);
            }
            return material;
        }
    }

    public static Material defaultMaterial
    {
        get
        {
            if (s_defaultMaterial == null)
            {
                s_defaultMaterial = Resources.Load("Shaders/UI/Default2", typeof(Material)) as Material;
            }
            return s_defaultMaterial;
        }
    }

    public override Material material
    {
        get
        {
            Material baseMaterial = this.baseMaterial;
            this.UpdateInternalState();
            if (base.m_IncludeForMasking && (base.m_MaskMaterial == null))
            {
                base.m_MaskMaterial = StencilMaterial.Add(baseMaterial, (((int) 1) << base.m_StencilValue) - 1);
                if (base.m_MaskMaterial != null)
                {
                    base.m_MaskMaterial.set_shaderKeywords(baseMaterial.get_shaderKeywords());
                    return base.m_MaskMaterial;
                }
            }
            return baseMaterial;
        }
        set
        {
            base.set_material(value);
        }
    }

    public override float preferredHeight
    {
        get
        {
            float num = base.get_preferredHeight();
            if (this.alphaTexLayout == ImageAlphaTexLayout.Vertical)
            {
                num *= 0.5f;
            }
            return num;
        }
    }

    public override float preferredWidth
    {
        get
        {
            float num = base.get_preferredWidth();
            if (this.alphaTexLayout == ImageAlphaTexLayout.Horizonatal)
            {
                num *= 0.5f;
            }
            return num;
        }
    }
}

