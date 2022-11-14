using Draw.src.Model;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Draw
{
	/// <summary>
	/// Класът, който ще бъде използван при управляване на диалога.
	/// </summary>
	public class DialogProcessor : DisplayProcessor
	{
		#region Constructor
		
		public DialogProcessor()
		{
		}
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Избран елемент.
		/// </summary>
		private List<Shape> selection= new List<Shape>();
		public List<Shape> Selection {
			get { return selection; }
			set { selection = value; }
		}
		
		/// <summary>
		/// Дали в момента диалога е в състояние на "влачене" на избрания елемент.
		/// </summary>
		private bool isDragging;
		public bool IsDragging {
			get { return isDragging; }
			set { isDragging = value; }
		}
		
		/// <summary>
		/// Последна позиция на мишката при "влачене".
		/// Използва се за определяне на вектора на транслация.
		/// </summary>
		private PointF lastLocation;
		public PointF LastLocation {
			get { return lastLocation; }
			set { lastLocation = value; }
		}
		private List<Shape> groupSelection = new List<Shape>();
		public List<Shape> GroupSelection
		{
			get { return groupSelection; }
			set { groupSelection = value; }
		}
		private List<Shape> helpSelectionGs = new List<Shape>();
		public List<Shape> HelpSelectionGs
		{
			get { return helpSelectionGs; }
			set { helpSelectionGs = value; }
		}
		private List<Shape> selectionC = new List<Shape>();
		public List<Shape> SelectionC
		{
			get { return selectionC; }
			set { selectionC = value; }
		}
		#endregion

		/// <summary>
		/// Добавя примитив - правоъгълник на произволно място върху клиентската област.
		/// </summary>
		public void AddRandomRectangle()
		{
			Random rnd = new Random();
			int x = rnd.Next(100,1000);
			int y = rnd.Next(100,600);
			
			RectangleShape rect = new RectangleShape(new Rectangle(x,y,100,200));
			rect.FillColor = Color.White;
			rect.StrokeColor = Color.Black;
			rect.BorderWidth = 2;

			ShapeList.Add(rect);
		}

		public void AddRandomEllipse()
		{
			Random rnd = new Random();
			int x = rnd.Next(100, 1000);
			int y = rnd.Next(100, 600);

			EllipseShape ellipse = new EllipseShape(new Rectangle(x, y, 100, 200));
			
			ellipse.FillColor = Color.White;
			ellipse.StrokeColor = Color.Red;
			ellipse.BorderWidth = 2;

			ShapeList.Add(ellipse);
		}

		public void AddRandomSquare()
		{
			Random rnd = new Random();
			int x = rnd.Next(100, 1000);
			int y = rnd.Next(100, 600);

			SquareShape square = new SquareShape(new Rectangle(x, y, 100, 100));

			square.FillColor = Color.White;
			square.StrokeColor = Color.Black;
			square.BorderWidth = 2;
			ShapeList.Add(square);
		}

		public void AddRandomCircle()
		{
			Random rnd = new Random();
			int x = rnd.Next(100, 1000);
			int y = rnd.Next(100, 600);

			CircleShape circle = new CircleShape(new Rectangle(x, y, 100, 100));

			circle.FillColor = Color.White;
			circle.StrokeColor = Color.Black;
			circle.BorderWidth = 2;
			ShapeList.Add(circle);
		}

		public void Copy()
		{
			selectionC.Clear();
			foreach (Shape item in selection)
			{
				selectionC.Add(item);
			}

		}


		public void Paste()
		{

			foreach (Shape item in selectionC)
			{
				if (item.GetType().Name == "EllipseShape")
				{
					AddRandomEllipse();
				}
				if (item.GetType().Name == "SquareShape")
				{
					AddRandomSquare();
				}
				if (item.GetType().Name == "RectangleShape")
				{
					AddRandomRectangle();
				}
				if (item.GetType().Name == "GroupShape")
				{
					groupSelection.Add(item);
					foreach (GroupShape group in groupSelection)
					{
						foreach(Shape ss in group.SubShapes)
                        {

							if (ss.GetType().Name == "EllipseShape")
							{
								AddRandomEllipse();
							}
							if (ss.GetType().Name == "SquareShape")
							{
								AddRandomSquare();
							}
							if (ss.GetType().Name == "RectangleShape")
							{
								AddRandomRectangle();
							}
						}
						
					}
					groupSelection.Remove(item);
				}

			}

		}
		//method for deleting all selected shapes
		public void DeleteSelected()
        {
			foreach(Shape item in selection)
            {
				helpSelectionGs.Add(item);
            }
			foreach(Shape item in helpSelectionGs)
            {
				selection.Remove(item);
				ShapeList.Remove(item);
            }
			helpSelectionGs.Clear();
        }
		//method for grouping
		public void Group()
        {
			
			float minx = float.PositiveInfinity;
			float maxx = float.NegativeInfinity;
			float miny = float.PositiveInfinity;
			float maxy = float.NegativeInfinity;
				
			foreach(Shape item in Selection)
            {
                if (minx > item.Location.X)
                {
					minx = item.Location.X;
                }
				if(maxx<item.Location.X+item.Width)
                {
					maxx = item.Location.X + item.Width;
                }
				if (miny > item.Location.Y)
				{
					miny = item.Location.Y;
				}
				if (maxy < item.Location.Y + item.Height)
				{
					maxy = item.Location.Y + item.Height;
				}
			}
			float width = maxx - minx;
			float height = maxy - miny;
			RectangleF s = new RectangleF(minx, miny, width, height);
			GroupShape group = new GroupShape(s);
            foreach (Shape item in Selection)
            {
				group.SubShapes.Add(item);
				ShapeList.Remove(item);
				
            }
            foreach (Shape item in group.SubShapes)
            {
				Selection.Remove(item);
				item.FillColor = Color.White;
				item.StrokeColor = Color.Black;
				item.BorderWidth = 2;
			
			}
			ShapeList.Add(group);
			Selection.Add(group);
		}
		//method for degrouping
		public void DeGroup()
		{
			foreach (Shape item in Selection)
			{
				helpSelectionGs.Add(item);
			}
			foreach (Shape item in helpSelectionGs)
            {
                if (item.GetType().Name == "GroupShape")
                {
					groupSelection.Add(item);
					foreach (GroupShape group in groupSelection)
					{
						foreach (Shape ss in group.SubShapes)
						{
							ShapeList.Add(ss);
							selection.Add(ss);
						}
						ShapeList.Remove(group);
						selection.Remove(group);
					}
					groupSelection.Remove(item);

				}
			}
			helpSelectionGs.Clear();
			
			
		}

		/// <summary>
		/// Проверява дали дадена точка е в елемента.
		/// Обхожда в ред обратен на визуализацията с цел намиране на
		/// "най-горния" елемент т.е. този който виждаме под мишката.
		/// </summary>
		/// <param name="point">Указана точка</param>
		/// <returns>Елемента на изображението, на който принадлежи дадената точка.</returns>
		public Shape ContainsPoint(PointF point)
		{
			for(int i = ShapeList.Count - 1; i >= 0; i--){
				if (ShapeList[i].Contains(point)){
				
						
					return ShapeList[i];
				}	
			}
			return null;
		}
		
		/// <summary>
		/// Транслация на избраният елемент на вектор определен от <paramref name="p>p</paramref>
		/// </summary>
		/// <param name="p">Вектор на транслация.</param>
		public void TranslateTo(PointF p)
		{
			if (selection.Count>0)
			{
				foreach(Shape item in Selection)
                {
                    Console.WriteLine(item.GetType().ToString());
					item.Location = new PointF(item.Location.X + p.X - lastLocation.X, item.Location.Y + p.Y - lastLocation.Y);
                    if (item.GetType().Name == "GroupShape")
                    {
						groupSelection.Add(item);
						foreach (GroupShape group in groupSelection)
						{
							foreach (Shape ss in group.SubShapes)
							{
								ss.Location = new PointF(
									ss.Location.X + p.X - lastLocation.X, 
									ss.Location.Y + p.Y - lastLocation.Y
									);

							}
						}
						groupSelection.Remove(item);
					}
				}
				
				lastLocation = p;
			}
		}

		public void TranslateToPoint(PointF p,int n,int j,int g)
		{
			

			if (selection.Count > 0)
			{
				if(selection[g].GetType().Name != "GroupShape")
                {
					for (int i = n; i < j; i++)
					{
						selection[i].Location = new PointF(selection[i].Location.X + p.X - lastLocation.X, selection[i].Location.Y + p.Y - lastLocation.Y);

					}
				}
				else
                {
					foreach(GroupShape group in groupSelection)
                    {
						for (int i = n; i < j; i++)
						{
							group.SubShapes[i].Location = new PointF(
								group.SubShapes[i].Location.X + p.X - lastLocation.X,
								group.SubShapes[i].Location.Y + p.Y - lastLocation.Y
								);

						}
					}
					
				}					
				
				lastLocation = p;
			}
		}

		//method for selectin all shapes
		public void SelectAll()
		{
			foreach (Shape item in ShapeList)
			{
				if (!selection.Contains(item))
				{
					selection.Add(item);
				}
			}
		}
		//method for unselection
		public void UnSelectAll()
		{
			foreach (Shape item in ShapeList)
			{
				if (selection.Contains(item))
				{
					selection.Remove(item);
				}
			}
		}
		//scaling method
		public void Scale(int w,int h,int n,int j,int g)
		{
			int m = j;
			for (int i=n ; i < j; i++)
			{
				if(selection[g].GetType().Name!="GroupShape")
                {
					selection[i].Width = w;
					selection[i].Height = h;
                }
                else
                {


					helpSelectionGs.Add(selection[g]);
						foreach (GroupShape group in groupSelection)
						{
							for (int p = n; p < m; p++)
							{
							group.SubShapes[p].Width = w;
							group.SubShapes[p].Height = h;
							
							}
						}
					helpSelectionGs.Remove(selection[g]);
					
					
				}
				
			}
			
		}
		//rotatingMethod
		public void RotateShape(float rAngle)
		{
			if (Selection.Count > 0)
			{
				foreach (Shape item in Selection)
				{
					if (item.GetType().Name == "GroupShape")
					{
						groupSelection.Add(item);
						foreach (GroupShape group in groupSelection)
						{
							foreach (Shape ss in group.SubShapes)
							{
								ss.ShapeAngle = rAngle;

							}
						}
						groupSelection.Remove(item);
					}
					else
					{
						item.ShapeAngle = rAngle;

					}

				}

			}
		}
		//Serialization method
		public void SerializeFile(object currentObject, string path = null)
		{

			Stream stream;
			IFormatter binaryFormatter = new BinaryFormatter();
			if (path == null)
			{
				stream = new FileStream("DrawFile.asd", FileMode.Create, FileAccess.Write, FileShare.None);
			}
			else
			{
				string preparePath = path + ".asd";
				stream = new FileStream(preparePath, FileMode.Create);

			}
			binaryFormatter.Serialize(stream, currentObject);
			stream.Close();
		}


		//Deserialization method
		public object DeSerializeFile(string path = null)
		{
			object currentObject;

			Stream stream;
			IFormatter binaryFormatter = new BinaryFormatter();
			if (path == null)
			{
				stream = new FileStream("DrawFile.asd", FileMode.Open);

			}
			else
			{
				stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
			}
			currentObject = binaryFormatter.Deserialize(stream);
			stream.Close();
			return currentObject;
		}


		public override void DrawShape(Graphics grfx, Shape item)
		{
			base.DrawShape(grfx, item);

			if (selection.Contains(item))
			{
				grfx.DrawRectangle(new Pen(Color.Gray),
					item.Location.X - 3,
					item.Location.Y - 3,
					item.Width + 6,
					item.Height + 6);

			}

		}
	}
}
