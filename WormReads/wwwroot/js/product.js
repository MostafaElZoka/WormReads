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
                                        <a onClick=Delete("/admin/product/delete/${data}") class="btn btn-secondary">
                                            <i class="bi bi-trash3"></i> Delete
                                        </a>
                                    </div>` //passing the url in delete button as an event that calls the delete function in JS
                }, "width": "10px",
                orderable: false,
                searchable: false
            }
        ]
    })
}

function Delete(url /*endpoint url*/) {
    Swal.fire({
        title: "Are you sure you want to delete this product?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                success: function (data) {
                    $('#myTable').DataTable().ajax.reload(); //refresh the table asynchronously
                    toastr.success(data.message);
                }
            })
        }
    });
}                                            