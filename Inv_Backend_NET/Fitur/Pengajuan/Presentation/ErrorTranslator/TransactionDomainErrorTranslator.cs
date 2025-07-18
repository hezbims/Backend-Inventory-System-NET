using Inventory_Backend_NET.Common.Presentation.Model;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.CancelTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.ConfirmTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.CreateTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Group;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.PrepareTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.RejectTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.UpdateTransaction;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Presentation.ErrorTranslator;

internal sealed class TransactionDomainErrorTranslator
{
    internal MyValidationError Translate(IBaseTransactionDomainError error)
    {
        return error switch
        {
            #region Cancel transaction
            CanNotCancelOtherUserTransaction =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_CANNOT_CANCEL_OTHER_USER_TRANSACTION",
                    Message: Resources.Transaction.can_not_cancel_other_user_transaction),
            CanNotCancelTransactionWithEmptyNotesError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_EMPTY_NOTES",
                    Message: Resources.Transaction.can_not_cancel_transaction_with_empty_notes),
            RejectedTransactionCanNotCanceled =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_REJECTED_TRANSACTION_CANNOT_CANCELED",
                    Message: Resources.Transaction.can_not_cancel_rejected_transaction),
            TransactionCanNotCanceledTwiceError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_CAN_NOT_CANCELED_TWICE",
                    Message: Resources.Transaction.transaction_can_not_canceled_twice),
            #endregion
            #region Common error
            NonAdminCanNotAssignSupplierGroupError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_NON_ADMIN_CANNOT_ASSIGN_TO_SUPPLIER_GROUP",
                    Message: Resources.Transaction.non_admin_can_not_assign_supplier_group),
            TransactionItemsShouldNotBeEmptyError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_ITEMS_CAN_NOT_BE_EMPTY",
                    Message: Resources.Transaction.transaction_items_must_have_1_item),
            TransactionItemsSizeMustBeSameError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_ITEMS_SIZE_MUST_BE_SAME",
                    Message: Resources.Transaction.transaction_items_size_must_be_same),
            ExpectedQuantityMustGreaterThanZeroError err =>
                new MyValidationError(
                    Code: $"DOM_TRANSACTION_ITEM_EXPECTED_QUANTITY_MUST_BE_POSITIVE-{err.Index}",
                    Message: Resources.Transaction.transaction_item_expected_quantity_must_be_positive),
            PreparedQuantityMustNotNegativeError err =>
                new MyValidationError(
                    Code: $"DOM_TRANSACTION_ITEM_PREPARED_QUANTITY_MUST_NOT_NEGATIVE-{err.Index}",
                    Message: Resources.Transaction.transaction_item_prepared_quantity_must_not_be_negative),
             PreparedQuantityCantBeGreaterThanExpectedQuantityError err =>
                 new MyValidationError(
                     Code: $"DOM_TRANSACTION_ITEM_PREPARED_QUANTITY_CANT_BE_GREATER_THAN_EXPECTED_QUANTITY-{err.Index}",
                     Message: Resources.Transaction.transaction_item_prepared_quantity_cant_be_greater_than_expected_quantity),
            #endregion
            #region Confirm transaction
            AdminCanNotConfirmTransactionError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_ADMIN_CANNOT_CONFIRM",
                    Message: Resources.Transaction.admin_can_not_confirm_transaction),
            NonAdminCanNotConfirmOtherUserTransaction =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_NON_ADMIN_CAN_NOT_CONFIRM_OTHER_USER_TRANSACTION",
                    Message: Resources.Transaction.non_admin_can_not_confirm_other_user_transaction),
            NonAdminCanOnlyConfirmPreparedTransaction =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_NON_ADMIN_CAN_ONLY_CONFIRM_PREPARED_TRANSACTION",
                    Message: Resources.Transaction.non_admin_can_only_confirm_prepared_transaction),
            #endregion
            #region Create transaction
                AdminMustNotAssignTransactionToAdminUserError =>
                    new MyValidationError(
                        Code: "DOM_TRANSACTION_ADMIN_CANT_ASSIGN_TRANSACTION_TO_OTHER_ADMIN",
                        Message: Resources.Transaction.admin_can_not_assign_to_other_admin),
                UserNonAdminShouldNotCreateTransactionOfTypeInError =>
                    new MyValidationError(
                        Code: "DOM_TRANSACTION_NON_ADMIN_CAN_NOT_CREATE_TRANSACTION_OF_TYPE_IN",
                        Message: Resources.Transaction.non_admin_can_not_create_transaction_of_type_in),
            #endregion
            #region Group
            GroupIdNotFound err =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_GROUP_ID_NOT_FOUND",
                    Message: String.Format(Resources.Transaction.group_id_not_found, err.Id)),
            #endregion
            #region Prepare transaction
            OnlyWaitingTransactionCanBePreparedError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_ONLY_WAITING_TRANSACTION_CAN_BE_PREPARED",
                    Message: Resources.Transaction.only_waiting_transaction_can_be_prepared),
            UserNonAdminShouldNotPrepareTransactionError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_NON_ADMIN_CAN_NOT_PREPARE_TRANSACTION",
                    Message: Resources.Transaction.non_admin_can_not_prepare_transaction),
            #endregion
            #region Reject transaction
            NonAdminIsNotAllowedToRejectTransactionError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_NON_ADMIN_CAN_NOT_REJECT_TRANSACTION",
                    Message: Resources.Transaction.non_admin_can_not_reject_transaction),
            OnlyWaitingAndPreparedTransactionCanBeRejectedError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_ONLY_WAITING_AND_PREPARED_TRANSACTION_CAN_BE_REJECTED",
                    Message: Resources.Transaction.only_waiting_and_prepared_transaction_can_be_rejected),
            RejectionNotesMustNotBeEmptyError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_REJECTION_NOTES_MUST_NOT_BE_EMPTY",
                    Message: Resources.Transaction.rejection_notes_must_not_be_empty),
            #endregion
            #region Update transaction
            AdminCanOnlyUpdatePreparedTransaction =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_ADMIN_CAN_ONLY_UPDATE_PREPARED_TRANSACTION",
                    Message: Resources.Transaction.admin_can_only_updated_prepared_transaction),
            NonAdminCanNotUpdateNonWaitingTransactionError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_NON_ADMIN_CAN_ONLY_UPDATE_WAITING_TRANSACTION",
                    Message: Resources.Transaction.non_admin_can_only_update_waiting_transaction),
            NonAdminCanOnlyUpdateTheirOwnTransactionError =>
                new MyValidationError(
                    Code: "DOM_TRANSACTION_NON_ADMIN_CAN_ONLY_UPDATE_THEIR_OWN_TRANSACTION",
                    Message: Resources.Transaction.non_admin_can_only_update_their_own_transaction),
            #endregion
            _ =>
                throw new ArgumentOutOfRangeException($"No translation provided for type : {error.GetType().Name}")
        };
    }
}