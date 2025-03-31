using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Wpf;

namespace Corte3D_JNavarro
{
    public partial class MainWindow : Window
    {
        private readonly Model3DGroup modeloEstatico;
        private readonly Model3DGroup modeloAnimado;
        private double tiempoActual = 0.2;
        private bool tiempoIncremento = true;
        private readonly DispatcherTimer temporizador;

        private readonly Model3DGroup modeloCaraGato;


        public MainWindow()
        {
            InitializeComponent();

            // Inicializar los modelos
            modeloEstatico = new Model3DGroup();
            modeloAnimado = new Model3DGroup();

            modeloCaraGato = new Model3DGroup();


            // Configurar el temporizador para la animación 4D
            temporizador = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            temporizador.Tick += Temporizador_Tick;
            temporizador.Start();

            // Cargar los gráficos en sus respectivas vistas
            viewport3DEstatico.Children.Add(new ModelVisual3D { Content = modeloEstatico });
            GenerarSuperficieEstatico();

            viewport3DAnimado.Children.Add(new ModelVisual3D { Content = modeloAnimado });

            viewport3DGato.Children.Add(new ModelVisual3D { Content = modeloCaraGato });
            CreateCone();

        }

        private void VisualizarGraficos_Click(object sender, RoutedEventArgs e)
        {
            // Ocultar la portada y mostrar las pestañas
            PortadaGrid.Visibility = Visibility.Hidden;
            GraficosTabControl.Visibility = Visibility.Visible;
        }

        // Método para generar el gráfico 3D estático Z = F(X, Y) con sus respectivos parametros
        private void GenerarSuperficieEstatico()
        {
            //Parametro de dibujado
            modeloEstatico.Children.Clear();
            double minX = -9, maxX = 9;
            double minY = -9, maxY = 9;
            int resolucion = 30;

            for (double x = minX; x < maxX; x += (maxX - minX) / resolucion)
            {
                for (double y = minY; y < maxY; y += (maxY - minY) / resolucion)
                {
                    double z1 = CalcularZ(x, y);
                    double z2 = CalcularZ(x + 0.5, y);
                    double z3 = CalcularZ(x + 0.5, y + 0.5);
                    double z4 = CalcularZ(x, y + 0.5);

                    var puntosSuperficie = new Point3DCollection
                    {
                        new Point3D(x, y, z1),
                        new Point3D(x + 0.5, y, z2),
                        new Point3D(x + 0.5, y + 0.5, z3),
                        new Point3D(x, y + 0.5, z4)
                    };

                    var geometria = new MeshGeometry3D
                    {
                        Positions = puntosSuperficie,
                        TriangleIndices = { 0, 1, 2, 2, 3, 0 }
                    };

                    // Material sólido (sin textura)
                    var material = new DiffuseMaterial(new SolidColorBrush(Colors.CornflowerBlue));
                    modeloEstatico.Children.Add(new GeometryModel3D(geometria, material));
                }
            }
        }
        
        private double CalcularZ(double x, double y)
        {//ECUACIÓN PARA GENERAR EL GRAFICO
            return Math.Sqrt(x * x + y * y) + 3 * Math.Cos(Math.Sqrt(x * x + y * y)) + 5;
        }

        // Método para generar el gráfico 4D animado Z = F(X, Y, T) y sus respectivos parametros
        private void GenerarSuperficieAnimada()
        { //Parametro de dibujado
            modeloAnimado.Children.Clear();
            double minX = -10, maxX = 10;
            double minY = -10, maxY = 10;
            int resolucion = 30;

            for (double x = minX; x < maxX; x += (maxX - minX) / resolucion)
            {
                for (double y = minY; y < maxY; y += (maxY - minY) / resolucion)
                {
                    double z1 = CalcularZAnimado(x, y, tiempoActual);
                    double z2 = CalcularZAnimado(x + 0.5, y, tiempoActual);
                    double z3 = CalcularZAnimado(x + 0.5, y + 0.5, tiempoActual);
                    double z4 = CalcularZAnimado(x, y + 0.5, tiempoActual);

                    var puntosSuperficie = new Point3DCollection
                    {
                        new Point3D(x, y, z1),
                        new Point3D(x + 0.5, y, z2),
                        new Point3D(x + 0.5, y + 0.5, z3),
                        new Point3D(x, y + 0.5, z4)
                    };

                    var geometria = new MeshGeometry3D
                    {
                        Positions = puntosSuperficie,
                        TriangleIndices = { 0, 1, 2, 2, 3, 0 }
                    };

                    // Material sólido (sin textura)
                    var material = new DiffuseMaterial(new SolidColorBrush(Colors.IndianRed));
                    modeloAnimado.Children.Add(new GeometryModel3D(geometria, material));
                }
            }
        }

        private double CalcularZAnimado(double x, double y, double t)
        { //Ecuación para generar el grafico
            return Math.Sqrt(x * x * t + y * y * t) + 3 * Math.Cos(Math.Sqrt(x * x * t + y * y * t)) + 5;
        }

        //Grafico extra (Otra forma de trabajar con nuestro componente, esta vez para hacer un Cono 3D)
        private void CreateCone()
        {
            modeloCaraGato.Children.Clear();

            // Definimos las propiedades del cono
            double altura = 5.0;  // Altura del cono
            double radioBase = 2.0;  // Radio de la base del cono
            int segmentos = 30;  // Número de segmentos para la base circular
            double anguloBase = Math.PI * 2 / segmentos;

            // Creamos el vértice superior del cono
            var verticeSuperior = new Point3D(0, 0, altura);

            // Creamos los puntos de la base circular del cono
            var puntosBase = new Point3DCollection();
            for (int i = 0; i < segmentos; i++)
            {
                double angulo = i * anguloBase;
                double x = radioBase * Math.Cos(angulo);
                double y = radioBase * Math.Sin(angulo);
                puntosBase.Add(new Point3D(x, y, 0));  // La base está en z = 0
            }

            // Generamos las caras del cono (cada cara es un triángulo entre el vértice superior y dos puntos consecutivos de la base)
            for (int i = 0; i < segmentos; i++)
            {
                int nextIndex = (i + 1) % segmentos;  // El siguiente punto para cerrar la base circular

                var cara = new MeshGeometry3D();

                // Añadimos los vértices de la cara
                cara.Positions.Add(verticeSuperior);  // Vértice superior
                cara.Positions.Add(puntosBase[i]);   // Punto en la base (i)
                cara.Positions.Add(puntosBase[nextIndex]);  // Punto siguiente en la base

                // Añadimos los índices para formar los triángulos
                cara.TriangleIndices.Add(0);
                cara.TriangleIndices.Add(1);
                cara.TriangleIndices.Add(2);

                // Material para la cara
                var material = new DiffuseMaterial(new SolidColorBrush(Colors.Orange));

                // Creamos el modelo 3D de la cara y lo añadimos al modelo
                modeloCaraGato.Children.Add(new GeometryModel3D(cara, material));
            }

            // Ahora agregamos la base circular
            var baseMesh = new MeshGeometry3D();
            for (int i = 0; i < segmentos; i++)
            {
                int nextIndex = (i + 1) % segmentos;

                baseMesh.Positions.Add(new Point3D(0, 0, 0));  // Centro de la base
                baseMesh.Positions.Add(puntosBase[i]);  // Punto actual de la base
                baseMesh.Positions.Add(puntosBase[nextIndex]);  // Punto siguiente de la base

                int idx = i * 3;
                baseMesh.TriangleIndices.Add(idx);
                baseMesh.TriangleIndices.Add(idx + 1);
                baseMesh.TriangleIndices.Add(idx + 2);
            }

            // Material para la base
            var baseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Brown));

            // Creamos el modelo 3D de la base y lo añadimos al modelo
            modeloCaraGato.Children.Add(new GeometryModel3D(baseMesh, baseMaterial));
        }

        //configuración de la variable "tiempo"
        private void Temporizador_Tick(object sender, EventArgs e)
        {
            if (tiempoIncremento)
            {
                tiempoActual += 0.1;
                if (tiempoActual >= 3.0) tiempoIncremento = false;
            }
            else
            {
                tiempoActual -= 0.1;
                if (tiempoActual <= 0.2) tiempoIncremento = true;
            }

            GenerarSuperficieAnimada();
        }
    }
}
