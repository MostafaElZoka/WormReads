$(document).ready(function () {
    loadTable();
})

function loadTable() {
    $('#myTable').DataTable({
        ajax: '/company/company/getall',
        columns: [
            { data: 'name', "width": "10%" },
            { data: 'state', "width": "10%" },
            { data: 'city', "width": "10%" },
            { data: 'streetAddress', "width": "15%" },
            { data: 'postalCode', "width": "10%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return`                                  
                    
                                    <div class="d-flex justify-content-center">    
                                     <div class="w-75 btn-group text-center">
                                        <a href="/company/company/upsert/${data}" class="btn btn-primary mx-2">
                                            <i class="bi bi-pencil"></i> Edit
                                        </a>
                                        <a onClick=Delete("/company/company/delete/${data}") class="btn btn-secondary">
                                            <i class="bi bi-trash3"></i> Delete
                                        </a>
                                    </div>
                                    </div>`
                },
                "width": "35%"
            }
        ],
        columnDefs: [
            {className: "text-center", targets: "_all"}
        ]
    });
}
function Delete(url /*endpoint url*/) {
    Swal.fire({
        title: "Are you sure?",
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
                    $('#myTable').DataTable().ajax.reload();
                    toastr.success(data.message);
                }
        })
        }
    })
} ;