using ProjectCreator.Models;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string jsonFilePath = "projects.json";
        List<Project> allProjects = new List<Project>();

        public MainWindow()
        {
            InitializeComponent();
            UpdateProjectsAndTags();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                if (File.Exists(openFileDialog.FileName))
                ImagePath.Text = openFileDialog.FileName;
            }
        }

        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {

            string json = File.Exists(jsonFilePath) ? File.ReadAllText(jsonFilePath) : "[]";
            var projects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Project>>(json);
            if (projects == null)
            {
                projects = new List<Project>();
            }

            string imagePath = ImagePath.Text;
            string imageName = System.IO.Path.GetFileName(imagePath);
            string newImagePath = System.IO.Path.Combine("Resources", imageName).Replace('\\','/');

            if (!Directory.Exists("Resources"))
            {
                Directory.CreateDirectory("Resources");
            }
            if (!File.Exists(imagePath))
            {
                MessageBox.Show($"Image [{imagePath}] does not exist. Please select a valid image.");
                return;
            }
            if (File.Exists(newImagePath))
            {
                if (newImagePath != imagePath)
                {
                    //create a messagebox to ask if the user wants to overwrite the existing image
                    var result = MessageBox.Show($"Image [{newImagePath}] already exists. Do you want to overwrite it?", "Overwrite Image", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        MessageBox.Show("Please select a different image.");
                        return;
                    }

                    File.Delete(newImagePath);
                    File.Copy(imagePath, newImagePath);
                    ImagePath.Text = newImagePath;
                }
            }
            else
            {                
                File.Copy(imagePath, newImagePath);
                ImagePath.Text = newImagePath;
            }

            var project = new Project
            {
                Title = Title.Text,
                ImagePath = ImagePath.Text,
                ImageAlt = AltImage.Text,
                Description = Description.Text,
                Link = (Link.Text == "" ? null : Link.Text),
                Tags = new List<string>()
            };

            if (Tags.Text != "")
            {
                var tags = Tags.Text.Split(';').ToList();
                tags = tags.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).ToList();
                project.Tags = tags;
            }

            var existingIndex = projects.FindIndex(p => p.Title == project.Title);
            if (existingIndex != -1)
            {
                projects[existingIndex] = project; // Replace the existing project
            }
            else
            {
                projects.Add(project);
            }

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(projects, Newtonsoft.Json.Formatting.Indented);
            UpdateProjectsAndTags();
            File.WriteAllText(jsonFilePath, jsonString);
            MessageBox.Show("Project saved successfully!");
        }

        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            if (AllProjects.SelectedItem != null)
            {
                var selectedProject = allProjects.FirstOrDefault(p => p.Title == AllProjects.SelectedItem.ToString());
                if (selectedProject != null)
                {
                    Title.Text = selectedProject.Title;
                    ImagePath.Text = selectedProject.ImagePath;
                    AltImage.Text = selectedProject.ImageAlt;
                    Description.Text = selectedProject.Description;
                    Link.Text = selectedProject.Link;
                    Tags.Text = string.Join(";", selectedProject.Tags);
                }
            }
        }

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            if (Tags.Text != "" && Tags.Text.Last() != ';')
            {
                Tags.Text += ";" + AllTags.SelectedItem.ToString();
            }
            else
            {
                Tags.Text += AllTags.SelectedItem.ToString();
            }
        }

        private void UpdateProjectsAndTags()
        {
            if (!File.Exists(jsonFilePath))
            {
                File.Create(jsonFilePath).Close();
            }
            var projects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Project>>(File.ReadAllText(jsonFilePath));
            if (projects != null)
            {
                allProjects = projects;
                AllProjects.Items.Clear();
                AllTags.Items.Clear();
                foreach (var project in projects)
                {
                    AllProjects.Items.Add(project.Title);
                    foreach (var tag in project.Tags)
                    {
                        if (!AllTags.Items.Contains(tag))
                        {
                            AllTags.Items.Add(tag);
                        }
                    }
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // Clear all fields
            Title.Text = "";
            ImagePath.Text = "";
            AltImage.Text = "";
            Description.Text = "";
            Link.Text = "";
            Tags.Text = "";
            AllProjects.SelectedItem = null;
            AllTags.SelectedItem = null;
            AllProjects.Items.Clear();
            AllTags.Items.Clear();
            allProjects.Clear();
            // Reinitialize the projects and tags
            UpdateProjectsAndTags();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            // Remove selected project
            if (AllProjects.SelectedItem != null)
            {
                var selectedProject = allProjects.FirstOrDefault(p => p.Title == AllProjects.SelectedItem.ToString());
                if (selectedProject != null)
                {
                    allProjects.Remove(selectedProject);
                    File.WriteAllText(jsonFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(allProjects, Newtonsoft.Json.Formatting.Indented));
                    UpdateProjectsAndTags();
                }
            }
        }
    }
}