$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $('#myTable').DataTable({
        ajax: '/Admin/Product/GetAll',
        columns: [
            { data: 'title', "width": "10px" },
            { data: 'author', "width": "10px" },
            { data: 'isbn', "width": "1px" },
            { data: 'price', "width": "10px" },
            { data: 'category.name', "width": "10px" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group ">
                                        <a href="/admin/product/upsert/${data}" class="btn btn-primary mx-2">
                                            <i class="bi bi-pencil"></i> Edit
                                        </a>
                                        <a href="/admin/product/delete/${data}" class="btn btn-secondary">
                                            <i class="bi bi-trash3"></i> Delete
                                        </a>
                                    </div>`
                }, "width": "10px",
                orderable: false,
                searchable: false
            }
        ]
    })
}    ;