$(document).ready(function () {

    /*
     * Find every table having:
     *
     * class="master-data-table"
     *
     * and automatically convert it into
     * a DataTable.
     */

    $('.master-data-table').each(function () {

        var tableElement = $(this);

        var tableId = tableElement.attr('id');

        /*
         * Page information ID
         *
         * Example:
         *
         * candidateTable
         *      ↓
         * candidatePageInfo
         *
         * organizationTable
         *      ↓
         * organizationPageInfo
         */

        var pageInfoId = tableId.replace(
            'Table',
            'PageInfo'
        );


        /*
         * Initialize DataTable
         */

        var masterTable = tableElement.DataTable({

            /* Records per page */

            pageLength: 5,


            /* Dropdown */

            lengthMenu: [
                [5, 10, 25, 50, 100],
                [5, 10, 25, 50, 100]
            ],


            /* Sort by first column */

            order: [
                [0, 'asc']
            ],


            /* Search */

            searching: true,


            /* Pagination */

            paging: true,


            /* Information */

            info: true,


            /*
             * Text
             */

            language: {

                search: "Search:",

                lengthMenu: "Show _MENU_ entries",

                info: "Showing _START_ to _END_ of _TOTAL_ records",

                infoEmpty: "Showing 0 to 0 of 0 records",

                zeroRecords: "No matching records found",

                emptyTable: "No records found",

                paginate: {

                    first: "First",

                    last: "Last",

                    next: "Next",

                    previous: "Previous"

                }

            }

        });


        /*
         * Page X of Y
         */

        function updatePageInfo() {

            var pageInfo =
                masterTable.page.info();


            var currentPage =
                pageInfo.page + 1;


            var totalPages =
                pageInfo.pages;


            if (totalPages === 0) {

                $('#' + pageInfoId)
                    .text('Page 0 of 0');

            }
            else {

                $('#' + pageInfoId)
                    .text(
                        'Page ' +
                        currentPage +
                        ' of ' +
                        totalPages
                    );

            }

        }


        /*
         * Initial page information
         */

        updatePageInfo();


        /*
         * Update after search,
         * pagination or sorting
         */

        masterTable.on('draw', function () {

            updatePageInfo();

        });

    });

});