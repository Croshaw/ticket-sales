namespace TicketSales.UI;

public static class CreateUtils
{
    public static DataGridView GenerateDGV(DockStyle dockStyle = DockStyle.None, params ReadOnlySpan<string> columns)
    {
        var dgv = new DataGridView()
        {
            Dock = dockStyle,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AllowUserToOrderColumns = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            ColumnHeadersVisible = true,
        };
        if (columns.Length == 0)
            return dgv;
        foreach (var column in columns)
            dgv.Columns.Add(null, column);
        return dgv;
    }

    public static Button CreateButton(string text, EventHandler? onClick = null)
    {
        var button = new Button()
        {
            Text = text,
        };
        if(onClick is not null)
            button.Click += onClick;

        return button;
    }
}