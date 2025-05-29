using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Kenshi_DnD
{
    /// <summary>
    /// Lógica de interacción para UserControl.xaml
    /// </summary>
    public partial class CombatWindow : UserControl
    {
        MainWindow mainWindow;
        ContentControl controller;
        Adventure myAdventure;
        Cursor[] cursors;
        DispatcherTimer timer;
        int seconds;
        Combat myCombat;
        ITurnable fighterTarget;
        PlayerInventory myInventory;
        Hero[] heroes;
        Monster[] monsters;
        Random rnd;
        ITurnable currentAttacker;
        public CombatWindow(MainWindow mainWindow,ContentControl controller, Cursor[] cursors, Random rnd, Adventure myAdventure, Monster[] monsters)
        {
            InitializeComponent();
            //Starts a timer
            seconds = 0;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            this.mainWindow = mainWindow;
            this.controller = controller;
            this.cursors = cursors;
            this.Cursor = cursors[0];
            this.rnd = rnd;
            this.myAdventure = myAdventure;

            myInventory = myAdventure.GetInventory();
            this.heroes = myAdventure.GetCurrentSquad();
            this.monsters = monsters;
            myCombat = new Combat(heroes, monsters, myAdventure,rnd, this);

            currentAttacker = myCombat.GetCurrentAttacker();

            FillItemTrees();
            UpdateGameStateUI();
            UpdateFightersGrid();
            UpdateFightersStatsGrid(null,null);
            UpdateNextTurnUI();

            if (currentAttacker is Monster)
            {
                DispatcherTimer timeToAttack = new DispatcherTimer();
                timeToAttack.Interval = TimeSpan.FromMilliseconds(600);
                timeToAttack.Tick += MonsterAttackTimer_Tick;
                timeToAttack.Start();
            }

        }
        private async void NextTurn(object sender, RoutedEventArgs e)
        {
            if (myCombat.GetGameState() != 0)
            {
                UpdateGameStateUI();
                MessageBox.Show("Ya terminó el combate");
                return;
            }
            if (currentAttacker is Hero && fighterTarget == null)
            {
                MessageBox.Show("Selecciona un monstruo para atacar");
                return;
            }

            await myCombat.NextTurn(fighterTarget);

            if(fighterTarget != null)
            {
                if (!fighterTarget.IsAlive())
                {
                    fighterTarget = null;
                }
            }
            
            currentAttacker = myCombat.GetCurrentAttacker();

            UpdateFightersStatsGrid(null,null);
            UpdateFightersGrid();
            UpdateNextTurnUI();
            FillItemTrees();
            if (myCombat.GetGameState() != 0)
            {
                UpdateGameStateUI();
                MessageBox.Show("Ya terminó el combate");
                return;
            }
            if (currentAttacker is Monster)
            {
                DispatcherTimer timeToAttack = new DispatcherTimer();
                timeToAttack.Interval = TimeSpan.FromMilliseconds(600);
                timeToAttack.Tick += MonsterAttackTimer_Tick;
                timeToAttack.Start();
            }
        }
        private void UpdateFightersGrid()
        {

            AllHeroes.Children.Clear();
            AllMonsters.Children.Clear();
            AllHeroes.RowDefinitions.Clear();
            AllMonsters.RowDefinitions.Clear();
            AllHeroes.ColumnDefinitions.Clear();
            AllMonsters.ColumnDefinitions.Clear();


            for (int i = 0; i < 1; i+=1)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star);
                AllHeroes.ColumnDefinitions.Add(column);
            }
            for (int i = 0; i < heroes.Length; i+=1)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                AllHeroes.RowDefinitions.Add(row);
            }

            for (int i = 0; i < 1; i+=1)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star);
                AllMonsters.ColumnDefinitions.Add(column);
            }
            for (int i = 0; i < monsters.Length; i+=1)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                AllMonsters.RowDefinitions.Add(row);
            }

            for (int i = 0; i < heroes.Length; i+=1)
            {
                Button button = new Button();
                //Put content with the hero name and a progress bar with its hp
                button.Content = FighterStackPanel(heroes[i]);
                button.MouseEnter += ChangeCursorWhenHover;
                button.Tag = heroes[i];             
                

                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.Click += SelectTarget;
                if (heroes[i] == currentAttacker)
                {
                    Border border = PutBorderOnCurrentAttacker(button);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, 0);
                    AllHeroes.Children.Add(border);
                }
                else
                {
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, 0);
                    AllHeroes.Children.Add(button);
                }
            }
            for (int i = 0; i < monsters.Length; i+=1)
            {
                Button button = new Button();

                button.Content = FighterStackPanel(monsters[i]);
                button.MouseEnter += ChangeCursorWhenHover;
                button.HorizontalAlignment = HorizontalAlignment.Right;
               
                if (monsters[i].IsAlive())
                {
                    button.Tag = monsters[i];
                    button.Click += SelectTarget;
                }

                if (monsters[i] == currentAttacker)
                {
                    Border border = PutBorderOnCurrentAttacker(button);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, 0);
                    AllMonsters.Children.Add(border);
                }
                else
                {
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, 0);
                    AllMonsters.Children.Add(button);
                }
            }

        }
        private void FillItemTrees()
        {

            UnselectedItems.Items.Clear();
            HeroItems.Items.Clear();

            TreeViewItem root = GenerateTreeView();
            TreeViewItem ranged = (TreeViewItem)root.Items[0];
            TreeViewItem melee = (TreeViewItem)root.Items[1];
            TreeViewItem notConsumable = (TreeViewItem)melee.Items[0];
            TreeViewItem consumable = (TreeViewItem)melee.Items[1];

            Item[] rangedArray = myInventory.GetRanged(1);
            Item[] meleeArray = myInventory.GetMelee(1);
            Item[] consumableArray = myInventory.GetConsumables(1);

            if (rangedArray.Length == 0)
            {
                ranged.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int i = 0; i < rangedArray.Length; i+=1)
                {
                    ranged.Items.Add(GenerateItem(rangedArray[i]));
                }
            }
            if (meleeArray.Length == 0)
            {
                notConsumable.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int i = 0; i < meleeArray.Length; i+=1)
                {
                    notConsumable.Items.Add(GenerateItem(meleeArray[i]));
                }
            }

            if (consumableArray.Length == 0)
            {
                consumable.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int i = 0; i < consumableArray.Length; i+=1)
                {
                    consumable.Items.Add(GenerateItem(consumableArray[i]));
                }
            }
            if (meleeArray.Length == 0 && consumableArray.Length == 0)
            {
                melee.Visibility = Visibility.Collapsed;
            }

            UnselectedItems.Items.Add(root);

            
            if (currentAttacker is Monster)
            {
                return;
            }
            root = GenerateTreeView();

            ranged = (TreeViewItem)root.Items[0];
            melee = (TreeViewItem)root.Items[1];
            notConsumable = (TreeViewItem)melee.Items[0];
            consumable = (TreeViewItem)melee.Items[1];

            Hero currentHero = (Hero)currentAttacker;
            root.Header = root.Header + " de " + currentHero.GetName();
            HeroInventory heroInventory = currentHero.GetInventory();
            rangedArray = heroInventory.GetRanged(2);
            meleeArray = heroInventory.GetMelee(2);
            consumableArray = heroInventory.GetConsumables(2);
            if (rangedArray.Length == 0)
            {
                ranged.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int i = 0; i < rangedArray.Length; i+=1)
                {
                    ranged.Items.Add(GenerateItem(rangedArray[i]));
                }
            }
            if (meleeArray.Length == 0)
            {
                notConsumable.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int i = 0; i < meleeArray.Length; i+=1)
                {
                    notConsumable.Items.Add(GenerateItem(meleeArray[i]));
                }
            }

            if (consumableArray.Length == 0)
            {
                consumable.Visibility = Visibility.Collapsed;
            }
            else
            {
                for (int i = 0; i < consumableArray.Length; i+=1)
                {
                    consumable.Items.Add(GenerateItem(consumableArray[i]));
                }
            }
            if(meleeArray.Length == 0 && consumableArray.Length == 0)
            {
                melee.Visibility = Visibility.Collapsed;
            }
            HeroItems.Items.Add(root);

        }
        private void UpdateFightersStatsGrid(object sender , EventArgs e)
        {
            
            double width = HeroStatsGrid.ActualWidth;

            Debug.WriteLine(width);
            
            HeroStatsGrid.Children.Clear();
            HeroStatsGrid.ColumnDefinitions.Clear();
            HeroStatsGrid.RowDefinitions.Clear();

            if (currentAttacker is Hero)
            {
                Hero hero = (Hero)currentAttacker;

                double biggestNum;
                double bruteForce = hero.GetStat(Stats.Stat.BruteForce);
                biggestNum = bruteForce;
                double dexterity = hero.GetStat(Stats.Stat.Dexterity);
                if (biggestNum < dexterity)
                {
                    biggestNum = dexterity;
                }
                double resistance = hero.GetStat(Stats.Stat.Resistance);
                if (biggestNum < resistance)
                {
                    biggestNum = resistance;
                }
                double agility = hero.GetStat(Stats.Stat.Agility);
                if (biggestNum < agility)
                {
                    biggestNum = agility;
                }

                double proportion = width / biggestNum;


                for (int i = 0; i < 4; i += 1)
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                    HeroStatsGrid.RowDefinitions.Add(rowDefinition);
                }
                for (int i = 0; i < 4; i += 1)
                {
                    System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                    TextBlock textBlock = new TextBlock();
                    rectangle.HorizontalAlignment = HorizontalAlignment.Left;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    rectangle.VerticalAlignment = VerticalAlignment.Stretch;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    textBlock.Margin = new Thickness(0, 2, 0, 2);
                    rectangle.Margin = new Thickness(0, 2, 0, 2);
                    rectangle.Stroke = Brushes.Black;
                    switch (i)
                    {
                        case 0:
                            {
                                rectangle.Width = proportion * bruteForce < 0 ? 0 : proportion * bruteForce;
                                rectangle.Fill = Brushes.SaddleBrown;
                                textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@FBT@: " + bruteForce));
                                textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Fuerza Bruta: " + bruteForce,
                                    "La fuerza bruta es usada para ataques físicos, y para definir el nivel de artes marciales.");
                                ToolTipService.SetInitialShowDelay(textBlock, 100);
                                break;
                            }
                        case 1:
                            {
                                rectangle.Width = proportion * dexterity < 0 ? 0 : proportion * dexterity;
                                rectangle.Fill = Brushes.SteelBlue;

                                textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@DST@: " + dexterity));
                                textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Destreza: " + dexterity,
                                    "La destreza es usada para apuntar en ataques a distancia, y para definir el nivel de artes marciales.");

                                break;
                            }
                        case 2:
                            {
                                rectangle.Width = proportion * resistance < 0 ? 0 : proportion * resistance;
                                rectangle.Fill = Brushes.Olive;
                                textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@RES@: " + resistance));
                                textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Resistencia " + resistance,
                                    "La resistencia es usada para defenderse de los ataques enemigos, se necesita tener dos puntos por \nencima de la fuerza del atacante " +
                                    "para cada posibilidad de defenderse un punto de ataque.");
                                break;
                            }
                        case 3:
                            {
                                rectangle.Width = proportion * agility < 0 ? 0 : proportion * agility;
                                rectangle.Fill = Brushes.Gold;
                                textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@AG@: " + agility));
                                textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Agilidad: " + agility,
                                    "La agilidad es usada para atacar más seguido que tus enemigos, determinar el número de ataques en \nartes marciales." +
                                    "\nTambién para saquear a tus compañeros...");
                                break;
                            }
                    }
                    Grid.SetRow(rectangle, i);
                    Grid.SetRow(textBlock, i);
                    //Textblock must be added last, otherwise it won't show on top of rectangle
                    HeroStatsGrid.Children.Add(rectangle);
                    HeroStatsGrid.Children.Add(textBlock);


                }
                TargetStatsGrid.Children.Clear();
                TargetStatsGrid.ColumnDefinitions.Clear();
                TargetStatsGrid.RowDefinitions.Clear();
                if (fighterTarget == null)
                {
                    return;
                }
                if (fighterTarget is Hero)
                {
                    hero = (Hero)fighterTarget;
                    bruteForce = hero.GetStat(Stats.Stat.BruteForce);
                    biggestNum = bruteForce;
                    dexterity = hero.GetStat(Stats.Stat.Dexterity);
                    if (biggestNum < dexterity)
                    {
                        biggestNum = dexterity;
                    }
                    resistance = hero.GetStat(Stats.Stat.Resistance);
                    if (biggestNum < resistance)
                    {
                        biggestNum = resistance;
                    }
                    agility = hero.GetStat(Stats.Stat.Agility);
                    if (biggestNum < agility)
                    {
                        biggestNum = agility;
                    }

                    proportion = width / biggestNum;


                    for (int i = 0; i < 4; i += 1)
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                        TargetStatsGrid.RowDefinitions.Add(rowDefinition);
                    }
                    for (int i = 0; i < 4; i += 1)
                    {
                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                        TextBlock textBlock = new TextBlock();
                        rectangle.HorizontalAlignment = HorizontalAlignment.Right;
                        textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                        rectangle.VerticalAlignment = VerticalAlignment.Stretch;
                        textBlock.VerticalAlignment = VerticalAlignment.Center;
                        textBlock.Margin = new Thickness(0, 2, 0, 2);
                        rectangle.Margin = new Thickness(0, 2, 0, 2);
                        rectangle.Stroke = Brushes.Black;
                        switch (i)
                        {
                            case 0:
                                {
                                    rectangle.Width = proportion * bruteForce < 0 ? 0 : proportion * bruteForce;
                                    rectangle.Fill = Brushes.SaddleBrown;
                                    textBlock.Inlines.AddRange(mainWindow.DecorateText(bruteForce + " :@9@FBT@ "));
                                    textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Fuerza Bruta: " + bruteForce,
                                        "La fuerza bruta es usada para ataques físicos, y para definir el nivel de artes marciales.");
                                    ToolTipService.SetInitialShowDelay(textBlock, 100);
                                    break;
                                }
                            case 1:
                                {
                                    rectangle.Width = proportion * dexterity < 0 ? 0 : proportion * dexterity;
                                    rectangle.Fill = Brushes.SteelBlue;

                                    textBlock.Inlines.AddRange(mainWindow.DecorateText(dexterity + " :@9@DST@"));
                                    textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Destreza: " + dexterity,
                                        "La destreza es usada para apuntar en ataques a distancia, y para definir el nivel de artes marciales.");

                                    break;
                                }
                            case 2:
                                {
                                    rectangle.Width = proportion * resistance < 0 ? 0 : proportion * resistance;
                                    rectangle.Fill = Brushes.Olive;
                                    textBlock.Inlines.AddRange(mainWindow.DecorateText(resistance + " :@9@RES@"));
                                    textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Resistencia " + resistance,
                                        "La resistencia es usada para defenderse de los ataques enemigos, se necesita tener dos puntos por \nencima de la fuerza del atacante " +
                                        "para cada posibilidad de defenderse un punto de ataque.");
                                    break;
                                }
                            case 3:
                                {
                                    rectangle.Width = proportion * agility < 0 ? 0 : proportion * agility;
                                    rectangle.Fill = Brushes.Gold;
                                    textBlock.Inlines.AddRange(mainWindow.DecorateText(agility + " :@9@AG@"));
                                    textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Agilidad: " + agility,
                                        "La agilidad es usada para atacar más seguido que tus enemigos, determinar el número de ataques en \nartes marciales." +
                                        "\nTambién para saquear a tus compañeros...");
                                    break;
                                }
                        }
                        Grid.SetRow(rectangle, i);
                        Grid.SetRow(textBlock, i);
                        //Textblock must be added last, otherwise it won't show on top of rectangle
                        TargetStatsGrid.Children.Add(rectangle);
                        TargetStatsGrid.Children.Add(textBlock);


                    }
                }
                else
                {
                    Monster monster = (Monster)fighterTarget;
                    bruteForce = monster.GetStrength();
                    biggestNum = bruteForce;
                    
                    resistance = monster.GetResistance();
                    if (biggestNum < resistance)
                    {
                        biggestNum = resistance;
                    }
                    agility = monster.GetAgility();
                    if (biggestNum < agility)
                    {
                        biggestNum = agility;
                    }
                    width = TargetStatsGrid.ActualWidth;
                    proportion = width / biggestNum;

                    for (int i = 0; i < 4; i += 1)
                    {
                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                        TargetStatsGrid.RowDefinitions.Add(rowDefinition);
                    }
                    for (int i = 0; i < 4; i += 1)
                    {
                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                        TextBlock textBlock = new TextBlock();
                        rectangle.HorizontalAlignment = HorizontalAlignment.Right;
                        textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                        rectangle.VerticalAlignment = VerticalAlignment.Stretch;
                        textBlock.VerticalAlignment = VerticalAlignment.Center;
                        textBlock.Margin = new Thickness(0, 2, 0, 2);
                        rectangle.Margin = new Thickness(0, 2, 0, 2);
                        rectangle.Stroke = Brushes.Black;
                        switch (i)
                        {
                            case 0:
                                {
                                    rectangle.Width = proportion * bruteForce < 0 ? 0 : proportion * bruteForce;
                                    rectangle.Fill = Brushes.SaddleBrown;
                                    textBlock.Inlines.AddRange(mainWindow.DecorateText(bruteForce + " :@9@FRZ@ "));
                                    textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Fuerza: " + bruteForce,
                                        "La fuerza de los enemigos es el daño total que hacen si no consigues defenderte.");
                                    ToolTipService.SetInitialShowDelay(textBlock, 100);
                                    break;
                                }
                            case 1:
                                {
                                    rectangle.Width = proportion * resistance < 0 ? 0 : proportion * resistance;
                                    rectangle.Fill = Brushes.Olive;
                                    textBlock.Inlines.AddRange(mainWindow.DecorateText(resistance + " :@9@RES@"));
                                    textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Resistencia " + resistance,
                                        "La resistencia del enemigo es usada para defenderse de todo tu daño total.");
                                    break;
                                }
                            case 2:
                                {
                                    rectangle.Width = proportion * agility < 0 ? 0 : proportion * agility;
                                    rectangle.Fill = Brushes.Gold;
                                    textBlock.Inlines.AddRange(mainWindow.DecorateText(agility + " :@9@AG@"));
                                    textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Agilidad: " + agility,
                                        "La agilidad de los enemigos es usada para esquivar tus ataques y para \ndeterminar su frecuencia de turno.");
                                    break;
                                }
                            case 3:
                                {

                                    if (monster.GetImmunity() != Immunities.Immunity.None)
                                    {
                                        rectangle.Fill = Brushes.White;
                                    }
                                    else
                                    {
                                        rectangle.Fill = Brushes.WhiteSmoke;
                                    }
                                    rectangle.Width = proportion * biggestNum;
                                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                                    rectangle.HorizontalAlignment = HorizontalAlignment.Center;
                                    textBlock.Inlines.AddRange(mainWindow.DecorateText(monster.GetImmunityDescription()));

                                    break;
                                }
                        }
                        Grid.SetRow(rectangle, i);
                        Grid.SetRow(textBlock, i);
                        //Textblock must be added last, otherwise it won't show on top of rectangle
                        TargetStatsGrid.Children.Add(rectangle);
                        TargetStatsGrid.Children.Add(textBlock);
                    }
                }
            }
            
            

        }
        private TreeViewItem GenerateTreeView()
        {
            TreeViewItem root = new TreeViewItem { Header = "🎒Inventario", IsExpanded = true, Foreground = Brushes.WhiteSmoke };

            TreeViewItem ranged = new TreeViewItem { Header = "🏹Objetos a distancia", IsExpanded = true, Foreground = Brushes.WhiteSmoke };
            TreeViewItem melee = new TreeViewItem { Header = "🤜Objetos melee", IsExpanded = true, Foreground = Brushes.WhiteSmoke };

            TreeViewItem notConsumable = new TreeViewItem { Header = "⚔️No consumibles", IsExpanded = true, Foreground = Brushes.WhiteSmoke };
            TreeViewItem consumable = new TreeViewItem { Header = "💊Consumibles", IsExpanded = true, Foreground = Brushes.WhiteSmoke };

            melee.Items.Add(notConsumable);
            melee.Items.Add(consumable);

            root.Items.Add(ranged);
            root.Items.Add(melee);

            return root;
        }
        public void UseItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)sender;
            if (selectedItem != null && selectedItem.Tag is Item item)
            {
                Debug.WriteLine("Selected item: " + item.GetName());
                if (currentAttacker is Hero)
                {
                    Debug.WriteLine("Current attacker is a hero");
                    Hero currentHero = (Hero)currentAttacker;
                    if (IsParentX(selectedItem, HeroItems))
                    {
                        Debug.WriteLine("Trying to remove item");
                        RemoveItemFromHero(currentHero, item);
                    }
                    else
                    {
                        Debug.WriteLine("Trying to use item");
                        if (currentHero.CanUseItem(item))
                        {
                            Debug.WriteLine("Item added!");
                            AddItemToHero(currentHero, item);
                        }
                    }
                    UpdateFightersStatsGrid(null,null);
                    FillItemTrees();
                    UpdateFightersGrid();
                    UpdateNextTurnUI();
                }
            }
        }
        private void MonsterAttackTimer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            if (myCombat.GetGameState() != 0)
            {
                MessageBox.Show("Ya terminó el combate");
                UpdateGameStateUI();
                return;
            }
            NextTurn(null, null);
        }
        private void SelectTarget(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (currentAttacker is Monster)
            {
                return;
            }
            if (fighterTarget == button.Tag)
            {
                fighterTarget = null;
                UpdateLogUI(currentAttacker.GetName() + " piensa en la inmortalidad de los cangrejos");
            }
            else
            {
                fighterTarget = (ITurnable)button.Tag;
                UpdateLogUI(currentAttacker.GetName() + " mira a " + fighterTarget.GetName());
            }
            UpdateFightersGrid();
            UpdateFightersStatsGrid(null,null);
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            seconds += 1;
            string time;
            if (seconds > 3599)
            {
                time = TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
            }
            else
            {
                time = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
            }
            TimerUI.Content = time;
        }
        private void AddItemToHero(Hero hero, Item item)
        {
            hero.AddItemToInventory(item);
        }
        private void RemoveItemFromHero(Hero hero, Item item)
        {
            hero.RemoveItemFromInventory(item);
        }
        private void UpdateNextTurnUI()
        {
            ITurnable[] nextTurns = myCombat.GetNTurns(6);

            NextTurnGrid.Children.Clear();
            NextTurnGrid.ColumnDefinitions.Clear();
            NextTurnGrid.RowDefinitions.Clear();
            
            for(int i = 0; i < 3; i += 1)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = GridLength.Auto;
                NextTurnGrid.ColumnDefinitions.Add(columnDefinition);
            }
            for(int i = 0; i < 2;  i += 1)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                NextTurnGrid.RowDefinitions.Add(rowDefinition);
            }

            for(int i = 0; i < 3; i += 1)
            {
                Border border = new Border();
                border.Background = Brushes.Snow;
                border.BorderThickness = new Thickness(2);
                border.CornerRadius = new CornerRadius(5);
                border.Padding = new Thickness(10, 0, 10, 0);
                border.Margin = new Thickness(1, 3, 1, 3);

                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 12;

                if (nextTurns[i] is Hero)
                {
                    Hero hero = (Hero)nextTurns[i];

                    textBlock.Inlines.AddRange(mainWindow.DecorateText($"@916@{i+1}.@   " + hero.GetNameAndTitle()));
                    border.BorderBrush = Brushes.AntiqueWhite;
                    border.Child = textBlock;

                }
                else
                {
                    Monster monster = (Monster)nextTurns[i];


                    textBlock.Inlines.AddRange(mainWindow.DecorateText($"@916@{i+1}.@  " + nextTurns[i].GetName()));
                    border.BorderBrush = mainWindow.GetBrushByNum(monster.GetFaction().GetFactionColor());
                    border.Child = textBlock;
                }
                Grid.SetColumn(border, i);
                Grid.SetRow(border, 0);

                NextTurnGrid.Children.Add(border);
            }
            for (int i = 0; i < 3; i += 1)
            {
                Border border = new Border();
                border.Background = Brushes.Snow;
                border.BorderThickness = new Thickness(2);
                border.CornerRadius = new CornerRadius(5);
                border.Padding = new Thickness(10,0,10,0);
                border.Margin = new Thickness(1, 3, 1, 3);


                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 12;


                if (nextTurns[i+3] is Hero)
                {
                    Hero hero = (Hero)nextTurns[i+3];

                    textBlock.Inlines.AddRange(mainWindow.DecorateText($"@916@{i+4}.@   " + hero.GetNameAndTitle()));
                    border.BorderBrush = Brushes.AntiqueWhite;
                    border.Child = textBlock;

                }
                else
                {
                    Monster monster = (Monster)nextTurns[i+3];


                    textBlock.Inlines.AddRange(mainWindow.DecorateText($"@916@{i+4}.@   " + nextTurns[i +3].GetName()));
                    border.BorderBrush = mainWindow.GetBrushByNum(monster.GetFaction().GetFactionColor());
                    border.Child = textBlock;
                }
                Grid.SetColumn(border, i);
                Grid.SetRow(border, 1);
                NextTurnGrid.Children.Add(border);
            }
        }
        private void UpdateLogUI(string message)
        {
            CurrentCombatInfo.Inlines.Clear();
            CurrentCombatInfo.Inlines.AddRange(mainWindow.DecorateText(message));
            InfoLog.Inlines.AddRange(mainWindow.DecorateText(message + "\n"));
            InfoLogScroll.ScrollToEnd();
        }
        public async Task UpdateLogUI(string message, int ms)
        {
            CurrentCombatInfo.Inlines.Clear();
            CurrentCombatInfo.Inlines.AddRange(mainWindow.DecorateText(message));

            InfoLog.Inlines.AddRange(mainWindow.DecorateText(message + "\n"));
            if (ms != 0)
            {
                await Task.Delay(ms);
            }

        }
        public async Task UpdateDicesUI(string message, int ms)
        {
            DicesUI.Inlines.Clear();
            DicesUI.Inlines.AddRange(mainWindow.DecorateText(message));
            await Task.Delay(ms);
        }
        public async Task UpdateCombatStatsUI(string message, int ms)
        {
            CombatStats.Inlines.Clear();
            CombatStats.Inlines.AddRange(mainWindow.DecorateText(message));
            await Task.Delay(ms);
        }
        private void UpdateGameStateUI()
        {
            int gameState = myCombat.GetGameState();

            switch (gameState)
            {

                case 0:
                    {
                        GameStateUI.Content = "En combate";
                        break;
                    }
                case 1:
                    {
                        FreeInventoryFromAllHeroes();
                        GameStateUI.Content = "¡Combate ganado!";
                        timer.Stop();
                        myAdventure.GainToken();
                        Faction faction = monsters[0].GetFaction();
                        if (faction.GetRespectByFighting())
                        {
                            faction.AddOrSubtractRelation(50);
                        }
                        else
                        {
                            faction.AddOrSubtractRelation(-20);
                        }
                            mainWindow.PageController.Content = new Zone(mainWindow, controller, cursors, rnd, myAdventure, myAdventure.GetCurrentRegion());
                        break;
                    }
                case -1:
                    {
                        FreeInventoryFromAllHeroes();
                        GameStateUI.Content = "¡Has sido abandonado a tu suerte!";
                        timer.Stop();
                        myAdventure.GainToken();
                        Faction faction = monsters[0].GetFaction();
                        if (faction.GetRespectByFighting())
                        {
                            faction.AddOrSubtractRelation(-20);
                        }
                        else
                        {
                            // Player gains relations if the faction is able to win him to avoid the player getting into fights he can't win for a while
                            faction.AddOrSubtractRelation(30);
                        }
                        mainWindow.PageController.Content = new Map(mainWindow, mainWindow.PageController, cursors, rnd, myAdventure);
                        break;
                    }
            }
        }
        private void FreeInventoryFromAllHeroes()
        {
            for(int i = 0; i < heroes.Length; i += 1)
            {
                heroes[i].FreeAllItems();
            }
        }
        private bool IsParentX(TreeViewItem item, TreeView parentToCheck)
        {
            do
            {
                if (item.Parent is TreeViewItem)
                {
                    item = (TreeViewItem)item.Parent;
                }
                else
                {
                    if (item.Parent == parentToCheck)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            } while (item.Parent != null);
            return false;
        }
        private StackPanel FighterStackPanel(Monster monster)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText(monster == fighterTarget ?
                "🎯 " + monster.GetName() : monster.GetName()));
            textBlock.ToolTip = mainWindow.HeaderToolTipThemer(monster.GetName(),monster.ToString());
            ToolTipService.SetInitialShowDelay(textBlock, 100);
            textBlock.Padding = new Thickness(2);
            stackPanel.Children.Add(textBlock);


            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

            textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText(monster.GetFaction().GetFactionName()));
            textBlock.ToolTip = mainWindow.ToolTipThemer(monster.GetFaction().GetFactionDescription());
            ToolTipService.SetInitialShowDelay(textBlock, 100);
            Grid.SetColumn(textBlock, 1);
            grid.Children.Add(textBlock);
           
            if (!monster.IsAlive())
            {
                textBlock = new TextBlock();
                textBlock.Inlines.AddRange(mainWindow.DecorateText("@1@Muerto@"));
                textBlock.ToolTip = mainWindow.ToolTipThemer("Podrías haber sido tú");
                ToolTipService.SetInitialShowDelay(textBlock, 100);
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                Grid.SetColumn(textBlock, 2);
                grid.Children.Add(textBlock);
            }
            else
            {
                if (monster == fighterTarget)
                {
                    LinearGradientBrush linearBrush = new LinearGradientBrush();
                    linearBrush.StartPoint = new System.Windows.Point(1, 1);
                    linearBrush.EndPoint = new System.Windows.Point(0, 0);

                    linearBrush.GradientStops.Add(new GradientStop(Colors.AntiqueWhite, 0.0));
                    linearBrush.GradientStops.Add(new GradientStop(Colors.White, 0.5));
                    linearBrush.GradientStops.Add(new GradientStop(Colors.Red, 0.9));
                    linearBrush.GradientStops.Add(new GradientStop(Colors.IndianRed, 1.2));
                    stackPanel.Background = linearBrush;
                }
                else
                {
                    stackPanel.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
            stackPanel.Children.Add(grid);
            
            return stackPanel;
        }
        private StackPanel FighterStackPanel(Hero hero)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            label.Content = hero.GetName();
            label.ToolTip = mainWindow.HeaderToolTipThemer(hero.GetName(), hero.ToString());
            ToolTipService.SetInitialShowDelay(label, 100);
            stackPanel.Children.Add(label);

            if(hero == fighterTarget)
            {
                label.Content = "🎯 " + hero.GetName();
                LinearGradientBrush linearBrush = new LinearGradientBrush();
                linearBrush.StartPoint = new System.Windows.Point(0, 1);
                linearBrush.EndPoint = new System.Windows.Point(0.8, 0);

                linearBrush.GradientStops.Add(new GradientStop(Colors.AntiqueWhite, 0.0));
                linearBrush.GradientStops.Add(new GradientStop(Colors.White, 0.6));
                linearBrush.GradientStops.Add(new GradientStop(Colors.Red, 0.9));
                linearBrush.GradientStops.Add(new GradientStop(Colors.IndianRed, 1.2));
                stackPanel.Background = linearBrush;
            }
            else
            {
                stackPanel.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (hero.IsAlive())
            {
                ProgressBar progressBar = new ProgressBar();
                progressBar.Minimum = 0;
                progressBar.Maximum = hero.GetToughness();
                progressBar.Value = hero.GetHp();
                progressBar.Background = new SolidColorBrush(Colors.DarkGray);
                progressBar.Foreground = new SolidColorBrush(Colors.DarkRed);
                progressBar.Height = 20;
                progressBar.Width = 150;
                progressBar.Margin = new Thickness(4, 2, 4, 2);
                progressBar.ToolTip = mainWindow.ToolTipThemer(hero.GetHp() + "/" + hero.GetToughness());
                ToolTipService.SetInitialShowDelay(progressBar, 100);
                stackPanel.Children.Add(progressBar);
            }
            else
            {
                System.Windows.Controls.Label deadLabel = new System.Windows.Controls.Label();
                deadLabel.Content = "Inconsciente...";
                deadLabel.ToolTip = mainWindow.ToolTipThemer("Solo una buena cura podrá salvarlo ahora...\n \nEsperemos ganes el combate");
                deadLabel.HorizontalAlignment = HorizontalAlignment.Center;
                stackPanel.Children.Add(deadLabel);
            }
            return stackPanel;
        }
        private Border PutBorderOnCurrentAttacker(Button button)
        {
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Red);
            border.BorderThickness = new Thickness(2);
            border.CornerRadius = new CornerRadius(5);
            button.Padding = new Thickness(2);
            border.HorizontalAlignment = (button.Tag is Hero ? HorizontalAlignment.Left : HorizontalAlignment.Right);
            LinearGradientBrush linearColorBrush = new LinearGradientBrush();
            linearColorBrush.StartPoint = new System.Windows.Point(0, 0);
            linearColorBrush.EndPoint = new System.Windows.Point(1, 0);
            linearColorBrush.GradientStops.Add(new GradientStop(Colors.White, 0.0));
            linearColorBrush.GradientStops.Add(new GradientStop(Colors.WhiteSmoke,1.0));
            button.Background = linearColorBrush;

            border.Child = button;
            return border;
        }
        
        //The use of this method is to change the cursor when hovering over a button
        //I do not like it, but it's the only way to prevent the cursor from changing when the button is being clicked
        //that isn't too complex
        private void ChangeCursorWhenHover(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (currentAttacker is Hero)
            {
                Hero hero = (Hero)currentAttacker;
                if (button.Tag is Monster)
                {
                    if (hero.GetInventory().AreRangedItems())
                    {
                        button.Cursor = cursors[2];
                    }
                    else
                    {
                        button.Cursor = cursors[1];
                    }
                }
                else
                {
                    if(button.Tag is Hero)
                    {
                        if (hero.GetInventory().AreConsumableItems())
                        {
                            button.Cursor = cursors[5];
                        }
                        else
                        {
                            Hero heroTag = (Hero)button.Tag;
                            if (!heroTag.IsAlive())
                            {
                                button.Cursor = cursors[3];
                            }
                        }
                    }
                    
                }
                
                
            }
        }
        //Generates the TreeViewItem for the inventories
        private TreeViewItem GenerateItem(Item item)
        {
            string name = item.GetName();
            if(item is RangedItem)
            {
                RangedItem rangedItem = (RangedItem)item;
                name += $" [{rangedItem.GetAmmoAndMaxAmmo()}]"; 
            }
            Border border = new Border();
            border.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e3e4c9"));
            border.BorderThickness = new Thickness(2);
            border.CornerRadius = new CornerRadius(5);
            border.Child = new TextBlock() { Text = name, FontSize = 14};
            border.Padding = new Thickness(10,0,10,0);
            border.Background = new SolidColorBrush(Colors.WhiteSmoke);


            TreeViewItem treeViewItem = new TreeViewItem();
            treeViewItem.Header = border;
            treeViewItem.Tag = item;
            treeViewItem.ToolTip = mainWindow.HeaderToolTipThemer(item.GetName(), item.ToString());
            ToolTipService.SetInitialShowDelay(treeViewItem, 700);
            treeViewItem.Cursor = cursors[3];
            treeViewItem.HorizontalAlignment = HorizontalAlignment.Center;
            treeViewItem.MouseLeftButtonUp += UseItem;
            treeViewItem.Margin = new Thickness(4);
            treeViewItem.FontSize = 12;
            return treeViewItem;
        }
    }
}

