# 📜 JavaScript DataTable

# ✅ DataTable Options

ordering: false, // Ordering False
paging: false, // Pagination False
searching: false // Searching False
scrollX: true, // Enables horizontal scrolling
autoWidth: false, // Prevents auto-adjusting column widths    
colReorder: true, // Column Drag To New Location
dom: '<"d-flex justify-content-between"lBf>rtip', 
fixedColumns: {
        leftColumns: 3, // Keep the first column fixed while scrolling
},
buttons: [
    {
        extend: 'excelHtml5', // Excel Export Button
        text: '<i class="bi bi-file-spreadsheet-fill"></i> Excel',
        title: 'Customer Receivable Details',
        exportOptions: {
            columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16] // Export only visible columns
        }
    }
],
responsive: false, // Ensures columns don't collapse


## For Searching Functionality Column wise
initComplete: function () {    
    $('#thead-tr-id input').on('keyup change', function () {
        var colIndex = $(this).closest("th").index();
        dataTable.column(colIndex).search(this.value).draw();
    });
},


let request={
    obj1:[], //Send List of Data
    string: "" // Send string
}

### Initialize dataTable variable at Page Load
### On OK Click or Fetching Data in Datatable Destroy dataTable variable.
dataTable.destroy();
dataTable = $("#Table_Id").DataTable({      
    "ajax": function (data, callback, settings) {           
        return service.method();
    },
    "columnDefs": [
        {
            targets: '_all', // apply to all columns
            className: 'align-middle' // replace with your class
        }
    ],
    "columns": [
        {
            "data": null,
            "render": function (data, type, full, meta) {
                return `${meta.row + 1}`
            }, width: "80px", class: "text-center"
        },
        {
            "render": function (data, type, full, meta) {
                return `<a class="btn btn-sm rounded-pill d-flex justify-content-center align-items-center" style="cursor:pointer">
                        <i class="mdi mdi-square-edit-outline text-success"></i>
                    </a>`;
            }, width: "50px", class: "text-center"
        },
        {
            "render": function (data, type, full, meta) {
                if (full.column_name == "01-Jan-1900") {
                    return ``;
                }
                else {
                    return `<span>${full.column_name}</span>`;
                }
            }, width: "120px", class: "text-center"
        },
        { "data": "column_name", width: "120px"}
    ],
    "language": {
        "emptyTable": "No Records found"
    },
    "width": "100%"
});