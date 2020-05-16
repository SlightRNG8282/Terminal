using System;
using System.Collections.Generic;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;

using System.IO;
using System.Reflection;

namespace Project1
{
	public class Sprite
	{
		
		// New!
		public float Rotation;
		
		static ShaderProgram shaderProgram;
		protected GraphicsContext graphics;		
		float[] vertices=new float[12];
		
		float[] texcoords = {
			0.0f, 0.0f,	// left top
			0.0f, 1.0f,	// left bottom
			1.0f, 0.0f,	// right top
			1.0f, 1.0f,	// right bottom
		};		
		
		float[] colors = {
			1.0f,	1.0f,	1.0f,	1.0f,	// left top
			1.0f,	1.0f,	1.0f,	1.0f,	// left bottom
			1.0f,	1.0f,	1.0f,	1.0f,	// right top
			1.0f,	1.0f,	1.0f,	1.0f,	// right bottom
		};
		
		const int indexSize = 4;
		ushort[] indices;
		
		VertexBuffer vertexBuffer;
		protected Texture2D texture;

		public Vector3 Position;
		public Vector2 Center;		
		public Vector2 Scale=Vector2.One;		
		protected Vector4 color=Vector4.One;
		float width,height;
		
		public float Width 
		{
			get { return width * Scale.X;}
			set { width = value; }
		}

		public float Height 
		{
			get { return height * Scale.Y;}
			set { height = value; }
		}

		public Sprite(GraphicsContext graphics, Texture2D texture)
		{
			if(shaderProgram == null)
			{
				shaderProgram = new ShaderProgram("/Application/shaders/Sprite.cgx");
				shaderProgram.SetUniformBinding(0, "u_WorldMatrix");	
			}
			
			if (texture == null)
			{
				throw new Exception("ERROR: texture is null.");
			}
			
			this.graphics = graphics;
			this.texture = texture;
			this.width = texture.Width;
			this.height = texture.Height;
			
			

			indices = new ushort[indexSize];
			indices[0] = 0;
			indices[1] = 1;
			indices[2] = 2;
			indices[3] = 3;
			
			vertexBuffer = new VertexBuffer(4, indexSize, VertexFormat.Float3, 
			                                VertexFormat.Float2, VertexFormat.Float4);
		}

		public void SetColor(Vector4 color)
		{
			SetColor(color.R, color.G, color.B, color.A);
		}
		
		public void SetColor(float r, float g, float b, float a)
		{
			this.color.R = r;
			this.color.G = g;
			this.color.B = b;
			this.color.A = a;
			
			for (int i = 0; i < colors.Length; i+=4)
			{
				colors[i] = r;
				colors[i+1] = g;
				colors[i+2] = b;
				colors[i+3] = a;
			}
		}
		#region TEXTURE_REGION
		public void SetTextureCoord(float x0, float y0, float x1, float y1)
		{
			texcoords[0] = x0 / texture.Width;	// left top u
			texcoords[1] = y0 / texture.Height; // left top v
			
			texcoords[2] = x0 / texture.Width;	// left bottom u
			texcoords[3] = y1 / texture.Height;	// left bottom v
			
			texcoords[4] = x1 / texture.Width;	// right top u
			texcoords[5] = y0 / texture.Height;	// right top v
			
			texcoords[6] = x1 / texture.Width;	// right bottom u
			texcoords[7] = y1 / texture.Height;	// right bottom v
		}
		
		public void SetTextureCoord(Vector2 topLeft, Vector2 bottomRight)
		{
			SetTextureCoord(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
		}
		
		public void SetTextureUV(float u0, float v0, float u1, float v1)
		{
			texcoords[0] = u0;	// left top u
			texcoords[1] = v0;	// left top v
			
			texcoords[2] = u0;	// left bottom u
			texcoords[3] = v1;	// left bottom v
			
			texcoords[4] = u1;	// right top u
			texcoords[5] = v0;	// right top v
			
			texcoords[6] = u1;	// right bottom u
			texcoords[7] = v1;	// right bottom v
		}
		#endregion TEXTURE_REGION
		
		public void Render()
		{
			// =============== BEGINNING OF NEW STUFF =======================
			float x, y;
			float cosRot = FMath.Cos(Rotation);
			float sinRot = FMath.Sin(Rotation);
			
			// Point 1 - top left
			x = -Width*Center.X;
			y = -Height*Center.Y;
			vertices[0]=Position.X + (float)(x*cosRot - y*sinRot);	// x0
			vertices[1]=Position.Y + (float)(x*sinRot + y*cosRot);	// y0
			vertices[2]=Position.Z;						// z0
			
			// Point 2 - bottom left
			x = -Width*Center.X;
			y = Height*(1.0f-Center.Y);
			vertices[3]=Position.X + (float)(x*cosRot - y*sinRot);	// x1
			vertices[4]=Position.Y + (float)(x*sinRot + y*cosRot);// y1
			vertices[5]=Position.Z;						// z1
			
			// Point 3 - top right
			x = Width*(1.0f-Center.X);
			y = -Height*Center.Y;
			vertices[6]=Position.X + (float)(x*cosRot - y*sinRot);	// x2
			vertices[7]=Position.Y + (float)(x*sinRot + y*cosRot);		// y2
			vertices[8]=Position.Z;							// z2
			
			// Point 4 - bottom right
			x = Width*(1.0f-Center.X);
			y = Height*(1.0f-Center.Y);
			vertices[9]=Position.X + (float)(x*cosRot - y*sinRot);	// x3
			vertices[10]=Position.Y + (float)(x*sinRot + y*cosRot);// y3
			vertices[11]=Position.Z;					// z3
			
			graphics.Enable(EnableMode.Blend);
			// "Original"
			graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
			
			// Something Cool
			//graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.DstAlpha);
			
			// Additive blending
		//	graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.One);
			
			// ===============  END OF NEW STUFF ================================
			
			graphics.SetShaderProgram(shaderProgram);
			
			vertexBuffer.SetVertices(0, vertices);
			vertexBuffer.SetVertices(1, texcoords);
			vertexBuffer.SetVertices(2, colors);
			
			vertexBuffer.SetIndices(indices);
			graphics.SetVertexBuffer(0, vertexBuffer);
			graphics.SetTexture(0, texture);
			

			Matrix4 screenMatrix = new Matrix4(
				 2.0f/graphics.Screen.Rectangle.Width,	0.0f,		0.0f, 0.0f,
				 0.0f,	 -2.0f/graphics.Screen.Rectangle.Height,	0.0f, 0.0f,
				 0.0f,	 0.0f, 1.0f, 0.0f,
				-1.0f, 1.0f, 0.0f, 1.0f
			);

			shaderProgram.SetUniformValue(0, ref screenMatrix);

			graphics.DrawArrays(DrawMode.TriangleStrip, 0, indexSize);
			graphics.Disable(EnableMode.Blend);
		}
	}
}

