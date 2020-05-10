using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using IndependentReserve.DataContracts.OrderBook.Enums;
using IndependentReserve.DataContracts.OrderBook.Models;
using IndependentReserve.DataContracts.OrderBook.Models.ModelExtensions;

namespace IndependentReserve.Worker.Worker.Containers
{
    internal class RefreshBufferFactory
    {

        private readonly ConcurrentDictionary<Guid, ExtraOrderDto> _buffer;
        private readonly Dictionary<Func<OrderChangeDto, bool>, Action<OrderChangeDto>> _updateProcessors;

        public RefreshBufferFactory(ConcurrentDictionary<Guid, ExtraOrderDto> buffer)
        {
            _buffer = buffer;
            _updateProcessors = new Dictionary<Func<OrderChangeDto, bool>, Action<OrderChangeDto>>
            {
                {
                    order => order.Event == OrderChangeEvent.NewOrder || order.Event == OrderChangeEvent.OrderChanged, ProcessAddOrUpdateOrder
                },
                {
                    order => order.Event == OrderChangeEvent.OrderCanceled, CancelOrder
                }
            };
        }

        private void CancelOrder(OrderChangeDto orderChangeDto)
        {
            if (_buffer.ContainsKey(orderChangeDto.Data.OrderGuid))
                _buffer.TryRemove(orderChangeDto.Data.OrderGuid, out _);
        }

        private void ProcessAddOrUpdateOrder(OrderChangeDto orderChangeDto)
        {
            _buffer.AddOrUpdate(orderChangeDto.Data.OrderGuid, orderId => new ExtraOrderDto
            {
                Guid = orderChangeDto.Data.OrderGuid,
                Price = orderChangeDto.Data.Price,
                Volume = orderChangeDto.Data.Volume,
                Nonce = orderChangeDto.Nonce,
                UpdateDateTime = DateTime.Now
            }, (orderId, order) =>
            {
                order.Nonce = orderChangeDto.Nonce;
                order.UpdateDateTime = DateTime.Now;
                return order;
            });
        }

        public void ProcessChange(OrderChangeDto orderChangeDto)
        {
            foreach (var processor in _updateProcessors)
            {
                if (processor.Key(orderChangeDto))
                {
                    processor.Value(orderChangeDto);
                    return;
                }
            }
        }

    }
}