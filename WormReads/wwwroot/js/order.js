    let ajaxUrl = '/Admin/Order/GetAll'
$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get("status");

    if (status) {
        ajaxUrl += `?status=${status}`
    }
    //var list = document.querySelectorAll('#list .list-group-item');

    //list.forEach(item => {
    //    item.addEventListener('click', function () {
    //        list.forEach(li => li.classList.remove('active'));
    //        this.classList.add('active');
    //    });
    //});

    loadDataTable();
});
function loadDataTable() {
    $('#myTable').DataTable({
        ajax: ajaxUrl,
        columns: [
            { data: 'id', "width": "10px" },
            { data: 'name', "width": "10px" },
            { data: 'phoneNumber', "width": "1px" },
            { data: 'user.email', "width": "10px" },
            { data: 'orderStatus', "width": "10px" },
            { data: 'orderTotal', "width": "10px" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group ">
                                        <a href="/admin/order/details/${data}" class="btn btn-primary mx-2">
                                            <i class="bi bi-pencil"></i> Edit
                                        </a>
                                    </div>` //passing the url in delete button as an event that calls the delete function in JS
                }, "width": "10px",
                orderable: false,
                searchable: false
            }
        ]
    })
}

//function Delete(url /*endpoint url*/) {
//    Swal.fire({
//        title: "Are you sure you want to delete this product?",
//        text: "You won't be able to revert this!",
//        icon: "warning",
//        showCancelButton: true,
//        confirmButtonColor: "#3085d6",
//        cancelButtonColor: "#d33",
//        confirmButtonText: "Yes, delete it!"
//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: url,
//                success: function (data) {
//                    $('#myTable').DataTable().ajax.reload(); //refresh the table asynchronously
//                    toastr.success(data.message);
//                }
//            })
//        }
//    });
//}                                           