using System.Collections.Immutable;
using System.Globalization;
using TicketSales.UI.Core;

namespace TicketSales.UI;

public partial class MainForm : Form
{
    private Logic _logic;
    public MainForm()
    {
        InitializeComponent();
        _logic = new Logic("db.json");
        _logic.Load();
        SetupMainLayout();
        Closing += async (s, e) =>
        {
            await _logic.Save();
        };
    }

    private void SetupMainLayout()
    {
        var tabPanel = new TabControl()
        {
            Dock = DockStyle.Fill
        };
        tabPanel.TabPages.Add(GetCulturalVenuesTabPage());
        tabPanel.TabPages.Add(GetEventTabPage());
        Controls.Add(tabPanel);
    }

    private TabPage GetCulturalVenuesTabPage()
    {
        var tabPage = new TabPage()
        {
            Text = "Культурные заведения"
        };
        CulturalVenue? currentObj = null;
        var currentId = -1;
        var dgv = CreateUtils.GenerateDGV(DockStyle.Fill, "Название", "Вид", "Адрес");
        
        foreach (var venue in _logic.CulturalVenues)
            dgv.Rows.Add(venue.Name, venue.Type.GetString(), venue.Address);
        
        var panel = new FlowLayoutPanel()
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight
        };
        
        var nameTb = new TextBox();
        nameTb.PlaceholderText = "Название";

        #region Вид

        var label = new Label()
        {
            Text = "Вид"
        };
        
        var typeCb = new ComboBox();
        var culturalTypes = Enum.GetValues<CulturalType>();
        typeCb.Items.AddRange(culturalTypes.Select(type => type.GetString()).Cast<object>().ToArray());

        var hz = new FlowLayoutPanel()
        {
            WrapContents = false,
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true
        };
        hz.Controls.Add(label);
        hz.Controls.Add(typeCb);

        #endregion
        
        var addressTb = new TextBox();
        addressTb.PlaceholderText = "Адрес";
        
        var isEdit = new CheckBox()
        {
            Text = "Редактировать",
            AutoSize = true
        };
        dgv.SelectionChanged += (s, e) =>
        {
            if (dgv.SelectedRows.Count > 0)
            {
                currentId = dgv.SelectedRows[0].Index;
                currentObj = _logic.CulturalVenues[currentId];
                if (!isEdit.Checked || currentObj is null) return;
                nameTb.Text = currentObj.Name;
                addressTb.Text = currentObj.Address;
                typeCb.SelectedIndex = culturalTypes.ToImmutableList().IndexOf(currentObj.Type);
            }
            else
            {
                currentId = -1;
                currentObj = null;
            }
        };

        var editButton = CreateUtils.CreateButton("Добавить", (sender, args) =>
        {
            if (string.IsNullOrEmpty(nameTb.Text))
            {
                MessageBox.Show("Впишите название культурного заведения");
                return;
            }

            if (typeCb.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите вид культурного заведения");
                return;
            }
            if (string.IsNullOrEmpty(addressTb.Text))
            {
                MessageBox.Show("Впишите адрес культурного заведения");
                return;
            }
            
            if (!isEdit.Checked)
            {
                var culturalVenue = new CulturalVenue()
                {
                    Name = nameTb.Text,
                    Address = addressTb.Text,
                    Type = CulturalTypeExtensions.ParseCulturalType(typeCb.SelectedItem?.ToString() ?? ""),
                };
                _logic.CulturalVenues.Add(culturalVenue);
                dgv.Rows.Add(nameTb.Text, typeCb.SelectedItem?.ToString() ?? "", addressTb.Text);
            }
            else
            {
                if (currentObj is null)
                    return;
                dgv.Rows[currentId].Cells[0].Value = currentObj.Name = nameTb.Text;
                dgv.Rows[currentId].Cells[1].Value = typeCb.SelectedItem?.ToString() ?? "";
                dgv.Rows[currentId].Cells[2].Value = currentObj.Address = addressTb.Text;
                currentObj.Type = CulturalTypeExtensions.ParseCulturalType(typeCb.SelectedItem?.ToString() ?? "");
            }

            nameTb.Clear();
            addressTb.Clear();
            typeCb.SelectedIndex = -1;
        });

        var removeButton = CreateUtils.CreateButton("Удалить", (_, _) =>
        {
            if (currentObj is null) return;
            _logic.CulturalVenues.Remove(currentObj);
            dgv.Rows.RemoveAt(currentId);
            currentId = -1;
            currentObj = null;
        });

        isEdit.CheckedChanged += (sender, args) =>
        {
            editButton!.Text = !isEdit.Checked ? "Добавить" : "Изменить";
            if (!isEdit.Checked || currentObj is null)
            {
                nameTb.Clear();
                addressTb.Clear();
                typeCb.SelectedIndex = -1;
            }
            else
            {
                nameTb.Text = currentObj.Name;
                addressTb.Text = currentObj.Address;
                typeCb.SelectedIndex = culturalTypes.ToImmutableList().IndexOf(currentObj.Type);
            }
        };
        
        panel.Controls.Add(nameTb);
        panel.Controls.Add(hz);
        panel.Controls.Add(addressTb);
        panel.Controls.Add(editButton);
        panel.Controls.Add(removeButton);
        panel.Controls.Add(isEdit);
        
        
        tabPage.Controls.Add(dgv);
        tabPage.Controls.Add(panel);
        
        return tabPage;
    }

    private TabPage GetEventTabPage()
    {
        var tabPage = new TabPage()
        {
            Text = "Культурные мероприятия"
        };
        Event? currentObj = null;
        var currentId = -1;
        var dgv = CreateUtils.GenerateDGV(DockStyle.Fill, "Название", "Описание", "Дата", "Время", "Кол-во мест", "Стоимость места");
        
        var panel = new FlowLayoutPanel()
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight
        };
        
        #region Культурное заведение

        var label = new Label()
        {
            Text = "Культурное заведение"
        };
        
        var culturalCb = new ComboBox();
        CulturalVenue? currentCultural = null;
        var culturals = _logic.CulturalVenues.Select(c => c.Name);
        culturalCb.Items.AddRange(culturals.ToArray<object>());

        var hz = new FlowLayoutPanel()
        {
            WrapContents = false,
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true
        };
        hz.Controls.Add(label);
        hz.Controls.Add(culturalCb);

        culturalCb.SelectedIndexChanged += (s, e) =>
        {
            dgv.Rows.Clear();
            if (culturalCb.SelectedIndex == -1)
            {
                currentCultural = null;
                return;
            }
            currentCultural = _logic.CulturalVenues[culturalCb.SelectedIndex]; 
            var events = currentCultural!.Events;
            foreach (var evnt in events)
                dgv.Rows.Add(evnt.Name, evnt.Description, evnt.Date, evnt.Time, evnt.Tickets.Count, evnt.Tickets.Count == 0 ? 0 : evnt.Tickets[0].Price);
            currentObj = null;
            currentId = -1;
        };

        #endregion
        
        var nameTb = new TextBox();
        nameTb.PlaceholderText = "Название";
        
        var descrTb = new TextBox();
        descrTb.PlaceholderText = "Описание";
        
        var date = new DateTimePicker();
        var time = new DateTimePicker()
        {
            Format = DateTimePickerFormat.Time,
        };
        
        var count = new NumericUpDown()
        {
            Minimum = 0,
            Maximum = 99999
        };
        
        var price = new TextBox();
        price.PlaceholderText = "Стоимость места";
        
        var isEdit = new CheckBox()
        {
            Text = "Редактировать",
            AutoSize = true
        };
        dgv.SelectionChanged += (s, e) =>
        {
            if (currentCultural is not null && dgv.SelectedRows.Count > 0)
            {
                currentId = dgv.SelectedRows[0].Index;
                currentObj = currentCultural.Events[currentId];
                if (!isEdit.Checked || currentObj is null) return;
                nameTb.Text = currentObj.Name;
                descrTb.Text = currentObj.Description;
                date.Value = currentObj.Date.ToDateTime(currentObj.Time);
                time.Value = date.Value;
                count.Value = currentObj.Tickets.Count;
                price.Text = currentObj.Tickets.Count == 0 ? "0" : currentObj.Tickets[0].Price.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                currentId = -1;
                currentObj = null;
            }
        };

        var editButton = CreateUtils.CreateButton("Добавить", (sender, args) =>
        {
            if (currentCultural is null)
            {
                MessageBox.Show("Выберите культурное заведение");
                return;
            }
            if (string.IsNullOrEmpty(nameTb.Text))
            {
                MessageBox.Show("Впишите название мероприятия");
                return;
            }
            if (string.IsNullOrEmpty(descrTb.Text))
            {
                MessageBox.Show("Впишите описание мероприятия");
                return;
            }
            if (string.IsNullOrEmpty(count.Text))
            {
                MessageBox.Show("Впишите кол-во билетов");
                return;
            }
            if (string.IsNullOrEmpty(price.Text))
            {
                MessageBox.Show("Впишите цену за билет");
                return;
            }
            
            if (!isEdit.Checked)
            {
                var evnt = new Event()
                {
                    Name = nameTb.Text,
                    Description = descrTb.Text,
                    Date = DateOnly.FromDateTime(date.Value),
                    Time = TimeOnly.FromDateTime(time.Value),
                    VenueId = culturalCb.SelectedIndex,
                    Tickets = []
                };
                for (var i = 0; i < count.Value; i++)
                {
                    evnt.Tickets.Add(new Ticket()
                    {
                        EventId = currentId,
                        Price = decimal.Parse(price.Text)
                    });
                }
                currentCultural!.Events.Add(evnt);
                dgv.Rows.Add(evnt.Name, evnt.Description, evnt.Date, evnt.Time, evnt.Tickets.Count, evnt.Tickets.Count == 0 ? 0 : evnt.Tickets[0].Price);
            }
            else
            {
                if (currentObj is null)
                    return;
                dgv.Rows[currentId].Cells[0].Value = currentObj.Name = nameTb.Text;
                dgv.Rows[currentId].Cells[1].Value = currentObj.Description = descrTb.Text;
                dgv.Rows[currentId].Cells[2].Value = currentObj.Date = DateOnly.FromDateTime(date.Value);
                dgv.Rows[currentId].Cells[3].Value = currentObj.Time = TimeOnly.FromDateTime(time.Value);
                var pricev = decimal.Parse(price.Text);
                if (count.Value >= currentObj.Tickets.Count)
                {
                    for (var i = 0; i < count.Value; i++)
                    {
                        currentObj.Tickets.Add(new Ticket()
                        {
                            EventId = currentId,
                            Price = pricev
                        });
                    }
                }
                else
                {
                    for (var i = currentObj.Tickets.Count - 1; i > count.Value; i--)
                        currentObj.Tickets.RemoveAt(i);
                }
                foreach (var currentObjTicket in currentObj.Tickets)
                {
                    currentObjTicket.Price = pricev;
                }
                dgv.Rows[currentId].Cells[4].Value = count.Value;
                dgv.Rows[currentId].Cells[5].Value = price.Text;
            }

            nameTb.Clear();
            descrTb.Clear();
            count.Value = 0;
            price.Clear();
        });

        var removeButton = CreateUtils.CreateButton("Удалить", (_, _) =>
        {
            if (currentObj is null || currentCultural is null) return;
            currentCultural.Events.Remove(currentObj);
            dgv.Rows.RemoveAt(currentId);
            currentId = -1;
            currentObj = null;
        });

        isEdit.CheckedChanged += (sender, args) =>
        {
            editButton!.Text = !isEdit.Checked ? "Добавить" : "Изменить";
            if (!isEdit.Checked || currentObj is null)
            {
                nameTb.Clear();
                descrTb.Clear();
                count.Value = 0;
                price.Clear();
            }
            else
            {
                nameTb.Text = currentObj.Name;
                descrTb.Text = currentObj.Description;
                date.Value = currentObj.Date.ToDateTime(currentObj.Time);
                time.Value = date.Value;
                count.Value = currentObj.Tickets.Count;
                price.Text = currentObj.Tickets.Count == 0 ? "0" : currentObj.Tickets[0].Price.ToString();
            }
        };
        
        panel.Controls.Add(hz);
        panel.Controls.Add(nameTb);
        panel.Controls.Add(descrTb);
        panel.Controls.Add(date);
        panel.Controls.Add(time);
        panel.Controls.Add(count);
        panel.Controls.Add(price);
        panel.Controls.Add(editButton);
        panel.Controls.Add(removeButton);
        panel.Controls.Add(isEdit);
        
        
        tabPage.Controls.Add(dgv);
        tabPage.Controls.Add(panel);
        
        return tabPage;
    }
}