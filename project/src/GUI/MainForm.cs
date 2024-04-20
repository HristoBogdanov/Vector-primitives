using Draw.src.Model;
using Draw.src.utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Draw
{
	/// <summary>
	/// Върху главната форма е поставен потребителски контрол,
	/// в който се осъществява визуализацията
	/// </summary>
	public partial class MainForm : Form
	{
		/// <summary>
		/// Агрегирания диалогов процесор във формата улеснява манипулацията на модела.
		/// </summary>
		private DialogProcessor dialogProcessor = new DialogProcessor();
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

		/// <summary>
		/// Изход от програмата. Затваря главната форма, а с това и програмата.
		/// </summary>
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Close();
		}
		
		/// <summary>
		/// Събитието, което се прихваща, за да се превизуализира при изменение на модела.
		/// </summary>
		void ViewPortPaint(object sender, PaintEventArgs e)
		{
			dialogProcessor.ReDraw(sender, e);
		}
		
		/// <summary>
		/// Бутон, който поставя на произволно място правоъгълник със зададените размери.
		/// Променя се лентата със състоянието и се инвалидира контрола, в който визуализираме.
		/// </summary>
		void DrawRectangleSpeedButtonClick(object sender, EventArgs e)
		{
			dialogProcessor.AddRandomRectangle();
			
			statusBar.Items[0].Text = "Последно действие: Рисуване на правоъгълник на случайно място";
			
			viewPort.Invalidate();
		}

		/// <summary>
		/// Прихващане на координатите при натискането на бутон на мишката и проверка (в обратен ред) дали не е
		/// щракнато върху елемент. Ако е така то той се отбелязва като селектиран и започва процес на "влачене".
		/// Промяна на статуса и инвалидиране на контрола, в който визуализираме.
		/// Реализацията се диалогът с потребителя, при който се избира "най-горния" елемент от екрана.
		/// </summary>
		void ViewPortMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            if (pickUpSpeedButton.Checked)
            {
                var selectedShape = dialogProcessor.ContainsPoint(e.Location);
                if (selectedShape != null)
                {
                    if (dialogProcessor.Selection.Contains(selectedShape))
                    {
                        ReturnLastColor(selectedShape);
                        dialogProcessor.Selection.Remove(selectedShape);
                        statusBar.Items[0].Text = "Последно действие: Перемахване на селекция на примитив";

                    }
                    else
                    {
                        ColorSelected(selectedShape);
                        dialogProcessor.Selection.Add(selectedShape);
                        statusBar.Items[0].Text = "Последно действие: Селекция на примитив";
                    }
                }

                dialogProcessor.IsDragging = true;
                dialogProcessor.LastLocation = e.Location;
                viewPort.Invalidate();
            }

            if (placeRectangleButton.Checked)
			{
                dialogProcessor.AddRectangleOnClick(e.Location);
				viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Поставяне на правоъгълник";
            }
		}

        private void ReturnLastColor (Shape shape)
        {
            if (shape is GroupShape grShape)
            {
                foreach (Shape sh in grShape.SubShapes)
                {
                    ReturnLastColor(sh);
                }
            }
            else
            {
                shape.FillColor = shape.LastColor;
            }
        }

        private void ColorSelected(Shape shape)
        {
            if (shape is GroupShape grShape)
            {
                foreach (Shape sh in grShape.SubShapes)
                {
                    ColorSelected(sh);
                }
            }
            else
            {
                shape.LastColor = shape.FillColor;
                shape.FillColor = Color.Red;
            }
        }

        /// <summary>
        /// Прихващане на преместването на мишката.
        /// Ако сме в режм на "влачене", то избрания елемент се транслира.
        /// </summary>
        void ViewPortMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (dialogProcessor.IsDragging) {
				if (dialogProcessor.Selection != null) statusBar.Items[0].Text = "Последно действие: Влачене";
				dialogProcessor.TranslateTo(e.Location);
				viewPort.Invalidate();
			}
		}

		/// <summary>
		/// Прихващане на отпускането на бутона на мишката.
		/// Излизаме от режим "влачене".
		/// </summary>
		void ViewPortMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			dialogProcessor.IsDragging = false;
		}

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomEllipse();

            statusBar.Items[0].Text = "Последно действие: Рисуване на Елипса на случайно място";

            viewPort.Invalidate();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
			if(colorDialog1.ShowDialog() == DialogResult.OK) 
			{ 
				if(dialogProcessor.Selection.Count > 0) 
				{
                    foreach (var shape in dialogProcessor.Selection)
                    {
                        ColorNested(shape, colorDialog1.Color);
                    }
                        statusBar.Items[0].Text = "Последно действие: Смяна на цвета на селектираните фигури.";
                    viewPort.Invalidate();
				}
			}
			dialogProcessor.Selection.Clear();
        }

        private void ColorNested(Shape shape, Color color)
        {
            if (shape is GroupShape grShape)
            {
                foreach (Shape sh in grShape.SubShapes)
                {
                    ColorNested(sh, color);
                }
            }
            else
            {
                shape.FillColor = color;
                shape.LastColor = color;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
			if(trackBar1.Value != 0)
			{
                if (dialogProcessor.Selection.Count > 0)
                {
					foreach(var shape in dialogProcessor.Selection)
					{
                        ChangeOpacity(shape);

                    }
                    viewPort.Invalidate();
                    statusBar.Items[0].Text = "Последно действие: Промяна на прозрачност";
                }
            }
        }

        private void ChangeOpacity(Shape shape)
        {
            if (shape is GroupShape grShape)
            {
                foreach (Shape sh in grShape.SubShapes)
                {
                    ChangeOpacity(sh);
                }
            }
            else
            {
                shape.Opacity = trackBar1.Value;
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
			textBox1.Text = trackBar1.Value.ToString();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.OK)
            {
                if (dialogProcessor.Selection.Count > 0)
                {
					foreach (var shape in dialogProcessor.Selection)
					{
                        OutlineShape(shape);
                    }
                    viewPort.Invalidate();
                }
            }
            dialogProcessor.Selection.Clear();
            statusBar.Items[0].Text = "Последно действие: Смяна на цвета на външнен контур на селектираните фигури.";
        }

        private void OutlineShape(Shape shape)
        {
            if (shape is GroupShape grShape)
            {
                foreach (Shape sh in grShape.SubShapes)
                {
                    OutlineShape(sh);
                }
            }
            else
            {
                shape.StrokeColor = colorDialog2.Color;
                shape.FillColor = shape.LastColor;
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (trackBar2.Value != 0)
            {
                if (dialogProcessor.Selection.Count > 0)
                {
					foreach (var shape in dialogProcessor.Selection)
					{
                        ChangeOutlineWidth(shape, trackBar2.Value);
                    }
                    viewPort.Invalidate();
                    statusBar.Items[0].Text = "Последно действие: Промяна на дебелината на контура";
                }
            }
        }

        private void ChangeOutlineWidth(Shape shape, int width)
        {
            if (shape is GroupShape grShape)
            {
                foreach (Shape sh in grShape.SubShapes)
                {
                    ChangeOutlineWidth(sh, width);
                }
            }
            else
            {
                shape.StrokeWidth = width;
            }
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            textBox2.Text = trackBar2.Value.ToString();
        }

        private void viewPort_Load(object sender, EventArgs e)
        {
			textBox1.Text = trackBar1.Value.ToString();
            textBox2.Text = trackBar2.Value.ToString();

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
			if(dialogProcessor.Selection != null)
			{
				dialogProcessor.RotateSelected(45);
                statusBar.Items[0].Text = "Последно действие: Завъртане на селектирани елементи на 45 градуса.";
                viewPort.Invalidate();
			}
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection != null && dialogProcessor.Selection.Count > 0)
            {
                dialogProcessor.ScaleSelected(1.5f, 1.5f);
                viewPort.Invalidate();
            }
            statusBar.Items[0].Text = "Последно действие: Увеличаване големината на фигура";
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
			dialogProcessor.GroupSelected();
			viewPort.Invalidate();
            statusBar.Items[0].Text = "Последно действие: Групиране";
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            dialogProcessor.UngroupSelected();
            viewPort.Invalidate();
            statusBar.Items[0].Text = "Последно действие: Разгрупиране";
        }

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
			Color c = Color.FromName(e.ClickedItem.ToString());

			if(dialogProcessor.Selection.Count > 0) 
			{ 
				foreach(var shape in dialogProcessor.Selection)
				{
                    ColorNested(shape, c);
                }
                dialogProcessor.Selection.Clear();
			}
            viewPort.Invalidate();
            statusBar.Items[0].Text = "Последно действие: Промяна на цвета на фигура";
        }

        private void groupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogProcessor.GroupSelected();
            viewPort.Invalidate();
            statusBar.Items[0].Text = "Последно действие: Групиране";
        }

        private void toolStripButton8_Click_1(object sender, EventArgs e)
        {
			pickUpSpeedButton.Checked = false;
        }

        public List<Shape> copied = new List<Shape>();
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
			if(e.Control && e.KeyCode == Keys.R)
			{
				dialogProcessor.AddRandomRectangle();
				viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Добавяне на правоъгълник на случайна позиция";
            }

            if (e.KeyCode == Keys.Escape)
            {
                foreach (Shape shape in dialogProcessor.Selection)
                {
                    ColorizeUnselected(shape);
                }
                dialogProcessor.Selection.Clear();
                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Зачистване на селекция";
            }

            void ColorizeUnselected(Shape shape)
            {
                if (shape is GroupShape grShape)
                {
                    foreach (Shape sh in grShape.SubShapes)
                    {
                        ColorizeUnselected(sh);
                    }
                }
                else
                {
                    shape.FillColor = shape.LastColor;
                }
            }

            if (e.Control && e.KeyCode == Keys.A)
			{
				dialogProcessor.Selection.AddRange(dialogProcessor.ShapeList);
				foreach (var shape in dialogProcessor.Selection)
				{
                    ColorizeSelected(shape);

                }
				viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Селектиране на всичко";
            }

            void ColorizeSelected(Shape shape)
            {
                if (shape is GroupShape grShape)
                {
                    foreach (Shape sh in grShape.SubShapes)
                    {
                        ColorizeSelected(sh);
                    }
                }
                else
                {
                    shape.LastColor = shape.FillColor;
                    shape.FillColor = Color.Red;
                }
            }

            if (e.KeyCode == Keys.D && e.Control)
            {
                if (dialogProcessor.Selection != null)
                {
                    foreach (var item in dialogProcessor.Selection)
                    {
                        dialogProcessor.ShapeList.Remove(item);
                    }
                    dialogProcessor.Selection.Clear();
                }
                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Изтриване на фигури";
            }


            if (e.KeyCode == Keys.C && e.Control)
            {
                copied.Clear();

                if (dialogProcessor.Selection.Count > 0)
                {
                    foreach (var shape in dialogProcessor.Selection)
                    {
                        var newShape = CopyShape(shape);
                        copied.Add(newShape);
                    }

                    dialogProcessor.Selection.Clear();
                    viewPort.Invalidate();
                    statusBar.Items[0].Text = "Последно действие: Копиране";
                }
            }

            if (e.Control && e.KeyCode == Keys.V)
            {
                dialogProcessor.ShapeList.AddRange(copied);
                dialogProcessor.Selection.Clear();
                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Поставяне";
            }

            if (e.KeyCode == Keys.Delete)
			{
				if(dialogProcessor.Selection.Count > 0)
				{
					foreach(var item in dialogProcessor.Selection)
					{
						dialogProcessor.ShapeList.Remove(item);
					}
                    dialogProcessor.Selection.Clear();
				}
                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Изтриване";
            }

            if (e.Control && e.KeyCode == Keys.G)
            {
                if (dialogProcessor.Selection.Count > 0)
                {
					dialogProcessor.GroupSelected();
					viewPort.Invalidate();
                    statusBar.Items[0].Text = "Последно действие: Групиране";
                }
            }

            if (e.Control && e.KeyCode == Keys.U)
			{
                if (dialogProcessor.Selection.Count > 0)
                {
                    dialogProcessor.UngroupSelected();
					viewPort.Invalidate();
                    statusBar.Items[0].Text = "Последно действие: Разгрупиране";
                }
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                if (dialogProcessor.Selection.Count == 0)
                {
                    MessageBox.Show("Моля селектирайте фигура за експортиране.", "Не е избрана фигура!");
                    return;
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Binary files (*.bin)|*.bin";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    ShapeSerializer.SerializeShapes(dialogProcessor.Selection, filePath);
                }
                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Запазване на фигура";
            }
        }

        private Shape CopyShape(Shape shape)
        {
            Type shapeType = shape.GetType();

            if (shape is GroupShape groupShape)
            {
                GroupShape newGroupShape = new GroupShape(groupShape.Rectangle);

                foreach (var subShape in groupShape.SubShapes)
                {
                    newGroupShape.SubShapes.Add(CopyShape(subShape));
                }

                //var newMatrix = groupShape.TransformationMatrix.Clone();
                //newGroupShape.TransformationMatrix = newMatrix;

                return newGroupShape;
            }
            else
            {
                Shape newShape = (Shape)Activator.CreateInstance(shapeType, new object[]
                {
                    new RectangleF(shape.Location.X, shape.Location.Y, shape.Width, shape.Height)
                });

                var newMatrix = shape.TransformationMatrix.Clone();
                newShape.TransformationMatrix = newMatrix;
                newShape.LastColor = shape.LastColor;
                newShape.FillColor = newShape.LastColor;
                newShape.StrokeColor = shape.StrokeColor;
                newShape.StrokeWidth = shape.StrokeWidth;
                newShape.Opacity = shape.Opacity;
                shape.FillColor = shape.LastColor;

                return newShape;
            }
        }

        private void groupToolStripMenuItem1_Click(object sender, EventArgs e)
        {
			if(dialogProcessor.Selection.Count > 1)
			{
				dialogProcessor.GroupSelected();
				viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Групиране";
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection != null)
            {
                dialogProcessor.RotateSelected(90);
                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Завъртане на селектирани елементи на 90 градуса";
            }
        }

        private void rotate90DegreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection != null)
            {
                dialogProcessor.RotateSelected(90);
                statusBar.Items[0].Text = "Последно действие: Завъртане на селектирани елементи на 90 градуса.";
                viewPort.Invalidate();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if (dialogProcessor.Selection != null)
			{
				foreach (var item in dialogProcessor.Selection)
				{
					dialogProcessor.ShapeList.Remove(item);
				}
                dialogProcessor.Selection.Clear();
			}
            viewPort.Invalidate();
            statusBar.Items[0].Text = "Последно действие: Изтриване на фигури";
        }

        private void ungroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if(dialogProcessor.Selection != null)
			{
				dialogProcessor.UngroupSelected();
				viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Разгрупиране";
            }
        }

        private void pickUpSpeedButton_Click(object sender, EventArgs e)
        {
			placeRectangleButton.Checked = false;
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection.Count == 0)
            {
                MessageBox.Show("Моля селектирайте фигура за експортиране.", "Не е избрана фигура!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Binary files (*.bin)|*.bin";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                ShapeSerializer.SerializeShapes(dialogProcessor.Selection, filePath);
            }
			viewPort.Invalidate();
            statusBar.Items[0].Text = "Последно действие: Запазване на фигура";
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Binary files (*.bin)|*.bin";
            openFileDialog.Title = "Select a Shape File to Import";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                List<Shape> deserializedShapes = ShapeSerializer.DeserializeShapes(filePath);

                dialogProcessor.ShapeList.AddRange(deserializedShapes);

                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Вмъкване на форма от външен носител";
            }
        }

        private void scaleDown_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection != null && dialogProcessor.Selection.Count > 0)
            {
                dialogProcessor.ScaleSelected(0.7f, 0.7f);
                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Намаляване размерите на фигура";
            }
        }

        private void exportShapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection.Count == 0)
            {
                MessageBox.Show("Моля селектирайте фигура за експортиране.", "Не е избрана фигура!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Binary files (*.bin)|*.bin";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                ShapeSerializer.SerializeShapes(dialogProcessor.Selection, filePath);
            }
            viewPort.Invalidate();
            statusBar.Items[0].Text = "Последно действие: Запазване на фигура";
        }

        private void importFromDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Binary files (*.bin)|*.bin";
            openFileDialog.Title = "Select a Shape File to Import";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                List<Shape> deserializedShapes = ShapeSerializer.DeserializeShapes(filePath);

                dialogProcessor.ShapeList.AddRange(deserializedShapes);

                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Вмъкване на форма от външен носител";
            }
        }

        private void rotate45ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection != null)
            {
                dialogProcessor.RotateSelected(45);
                statusBar.Items[0].Text = "Последно действие: Завъртане на селектирани елементи на 45 градуса.";
                viewPort.Invalidate();
            }
        }

        private void ungroupToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(dialogProcessor.Selection.Count > 0)
            {
                dialogProcessor.UngroupSelected();
                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Разрупиране";
            }
        }

        private void rotate45ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomStar();

            statusBar.Items[0].Text = "Последно действие: Рисуване на Звезда на случайно място";

            viewPort.Invalidate();
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomPolygon();

            statusBar.Items[0].Text = "Последно действие: Рисуване на многоъгълник на случайно място";

            viewPort.Invalidate();
        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomLine();

            statusBar.Items[0].Text = "Последно действие: Рисуване на отсечка на случайно място";

            viewPort.Invalidate();
        }

        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomDot();

            statusBar.Items[0].Text = "Последно действие: Рисуване на точка на случайно място";

            viewPort.Invalidate();
        }

        private void rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection != null)
            {
                dialogProcessor.RotateSelected(90);
                statusBar.Items[0].Text = "Последно действие: Завъртане на селектирани елементи на 90 градуса.";
                viewPort.Invalidate();
            }
        }

        private void scaleUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection != null && dialogProcessor.Selection.Count > 0)
            {
                dialogProcessor.ScaleSelected(1.5f, 1.5f);
                viewPort.Invalidate();
            }
            statusBar.Items[0].Text = "Последно действие: Увеличаване големината на фигура";
        }

        private void scaleDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection != null && dialogProcessor.Selection.Count > 0)
            {
                dialogProcessor.ScaleSelected(0.5f, 0.5f);
                viewPort.Invalidate();
                statusBar.Items[0].Text = "Последно действие: Намаляване размерите на фигура";
            }
        }

        private void drawDotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomDot();

            statusBar.Items[0].Text = "Последно действие: Рисуване на точка на случайно място";

            viewPort.Invalidate();
        }

        private void drawLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomLine();

            statusBar.Items[0].Text = "Последно действие: Рисуване на отсечка на случайно място";

            viewPort.Invalidate();
        }

        private void drawRectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomRectangle();

            statusBar.Items[0].Text = "Последно действие: Рисуване на правоъгълник на случайно място";

            viewPort.Invalidate();
        }

        private void drawElipseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomEllipse();

            statusBar.Items[0].Text = "Последно действие: Рисуване на елипса на случайно място";

            viewPort.Invalidate();
        }

        private void drawPentagonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomPolygon();

            statusBar.Items[0].Text = "Последно действие: Рисуване на многоъгълник на случайно място";

            viewPort.Invalidate();
        }

        private void drawStarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomStar();

            statusBar.Items[0].Text = "Последно действие: Рисуване на звезда на случайно място";

            viewPort.Invalidate();
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dialogProcessor.Selection != null)
            {
                foreach (var item in dialogProcessor.Selection)
                {
                    dialogProcessor.ShapeList.Remove(item);
                }
                dialogProcessor.Selection.Clear();
            }
            viewPort.Invalidate();
            statusBar.Items[0].Text = "Последно действие: Изтриване на фигури";
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            dialogProcessor.AddRandomLineRectangle();

            statusBar.Items[0].Text = "Последно действие: Рисуване на правоъгълник с линии на случайно място";

            viewPort.Invalidate();
        }
    }
}
