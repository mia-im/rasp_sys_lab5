using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace lab5_2
{
    public partial class Form1 : Form
    {
        static int[] m;

        public Form1()
        {
            InitializeComponent();
            textBox1.ScrollToCaret();
        }

        class SortOne
        {
            List<int>[] buckets;
            int j;
            int p;
            int[] mas;
            int min;
            int max;
            double n;
            int x;
            public SortOne(List<int>[] buckets, int j, int p, int[] mas, int min, int max)
            {
                this.buckets = buckets;
                this.j = j;
                this.p = p;
                this.mas = mas;
                this.min = min;
                this.max = max;
                n = mas.Length;
                x = 0;
            }
            public void BucketSort()
            {
                int i0 = Convert.ToInt32(Math.Ceiling((n - j) / p));
                for (int i = 0; i < j; i++)
                {
                    x += Convert.ToInt32(Math.Ceiling((n - i) / p));
                }
                for (int i = x; i < (x + i0); i++)
                {
                    int idx = (mas[i] - min) / (max - min);
                    buckets[idx].Add(mas[i]);
                }
            }
        }

        public void BucketSort(int[] mas, int kBucket)
        {
            // массив корзин
            List<int>[] buckets = new List<int>[kBucket];
            // каждую корзину проинициализировать
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new List<int>();
            }
            // найти диапазон значений в массиве-источнике
            int min = int.MaxValue;
            int max = -int.MaxValue;
            for (int i = 0; i < mas.Length; i++)
            {
                min = Math.Min(min, mas[i]);
                max = Math.Max(max, mas[i]);
            }
            for (int i = 0; i < mas.Length; i++)
            {
                // вычисление индекса корзины
                int idx = (mas[i] - min) / (max - min);
                // добавление элемента в соответствующую корзину
                buckets[idx].Add(mas[i]);
            }
            // сортировка корзин
            Parallel.For(0, kBucket, i => buckets[i].Sort());
            // собираем отсортированные элементы обратно в массив-источник
            var index = 0;
            for (var i = 0; i < kBucket; i++)
            {
                for (var j = 0; j < buckets[i].Count; j++)
                {
                    mas[index++] = buckets[i][j];
                }                    
            }                   
        }

        public void BucketSortWithTreads(int[] mas, int k, int kBucket)
        {
            List<int>[] buckets = new List<int>[kBucket];
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new List<int>();
            }
            int min = int.MaxValue;
            int max = -int.MaxValue;
            for (int i = 0; i < mas.Length; i++)
            {
                min = Math.Min(min, mas[i]);
                max = Math.Max(max, mas[i]);
            }
            Thread[] threads = new Thread[k];
            SortOne[] sorts = new SortOne[k];
            //Создание объектов SortOne,
            //передаваемых создаваемым потокам
            for (int i = 0; i < k; i++)
            {
                sorts[i] = new SortOne(buckets, i, k, mas, min, max);
                threads[i] = new Thread(sorts[i].BucketSort);
                threads[i].Start();
            }
            //Синхронизация
            for (int i = 0; i < k; i++)
            {
                threads[i].Join();
            }
            Parallel.For(0, kBucket, i => buckets[i].Sort());
            var index = 0;
            for (var i = 0; i < kBucket; i++)
            {
                for (var j = 0; j < buckets[i].Count; j++)
                {
                    mas[index++] = buckets[i][j];
                }
            }
        }

        public void BucketSortWithTasks(int[] mas, int k, int kBucket)
        {
            List<int>[] buckets = new List<int>[kBucket];
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new List<int>();
            }
            int min = int.MaxValue;
            int max = -int.MaxValue;
            for (int i = 0; i < mas.Length; i++)
            {
                min = Math.Min(min, mas[i]);
                max = Math.Max(max, mas[i]);
            }
            Task[] tasks = new Task[k];
            SortOne[] sorts = new SortOne[k];
            //Создание объектов SortOne,
            //передаваемых создаваемым задачам
            for (int i = 0; i < k; i++)
            {
                sorts[i] = new SortOne(buckets, i, k, mas, min, max);
                tasks[i] = new Task(sorts[i].BucketSort);
                tasks[i].Start();
            }
            //Синхронизация
            for (int i = 0; i < k; i++)
            {
                tasks[i].Wait();
            }
            Parallel.For(0, kBucket, i => buckets[i].Sort());
            var index = 0;
            for (var i = 0; i < kBucket; i++)
            {
                for (var j = 0; j < buckets[i].Count; j++)
                {
                    mas[index++] = buckets[i][j];
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                textBox2.Text = filename;
                m = File.ReadAllText(filename, Encoding.Default).Split(' ').Select(x => int.Parse(x)).ToArray();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(textBox3.Text);
            int kBucket = Convert.ToInt32(textBox5.Text);
            int[] arr = new int[n];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = m[i];
            DateTime dt1 = DateTime.Now;
            BucketSort(arr, kBucket);
            DateTime dt2 = DateTime.Now;
            textBox6.Text = ((dt2 - dt1).TotalMilliseconds).ToString() + " c";
            textBox1.Text = (String.Join(" ", arr));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(textBox3.Text);
            int k = Convert.ToInt32(textBox4.Text);
            int kBucket = Convert.ToInt32(textBox5.Text);
            int[] arr = new int[n];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = m[i];
            DateTime dt1 = DateTime.Now;
            BucketSortWithTreads(arr, k, kBucket);
            DateTime dt2 = DateTime.Now;
            textBox6.Text = ((dt2 - dt1).TotalMilliseconds).ToString() + " c";
            textBox1.Text = (String.Join(" ", arr));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int n = Convert.ToInt32(textBox3.Text);
            int k = Convert.ToInt32(textBox4.Text);
            int kBucket = Convert.ToInt32(textBox5.Text);
            int[] arr = new int[n];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = m[i];
            DateTime dt1 = DateTime.Now;
            BucketSortWithTasks(arr, k, kBucket);
            DateTime dt2 = DateTime.Now;
            textBox6.Text = ((dt2 - dt1).TotalMilliseconds).ToString() + " c";
            textBox1.Text = (String.Join(" ", arr));
        }
    }
}